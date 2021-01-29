using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace CH.Project.QueryCommont
{
    public static class GenerateQueryExpression
    {
        /// <summary>
        /// 生成lamba表达式
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="queryModule"></param>
        /// <returns></returns>
        public static Expression<Func<TEntity, bool>> GenerateExpressionCore<TEntity>(this IQueryModule<TEntity> queryModule) where TEntity : class
        {
            if (queryModule == null)
            {
                return null;
            }
            var type = queryModule.GetType();
            var param = Expression.Parameter(typeof(TEntity), "u");
            Expression body = null;
            foreach (PropertyInfo property in type.GetProperties())
            {
                if (property == null)
                {
                    continue;
                }
                var propertyName = property.Name;
                var propertyValue = property.GetValue(queryModule);
                if (propertyValue == null)
                {
                    continue;
                }
                if (propertyValue is string)
                {
                    propertyValue = string.IsNullOrEmpty((string)propertyValue) ? string.Empty : propertyValue.ToString().Trim();
                }
                Expression sub = null;
                foreach (var attribute in property.GetCustomAttributes(typeof(QueryModeAttribute), true))
                {

                    var propertyPath = ((QueryModeAttribute)attribute).PropertyPaths;
                    if (propertyPath == null || propertyPath.Length == 0)
                    {
                        propertyPath = new[] { property.Name };
                    }
                    var experssion = CreateQueryExpression(param, propertyValue, propertyPath, ((QueryModeAttribute)attribute).Compare);
                    if (experssion != null)
                    {
                        sub = sub == null ? experssion : Expression.Or(sub, experssion);
                    }
                }
                if (sub != null)
                {
                    body = body == null ? sub : Expression.And(body, sub);
                }
            }
            if (body != null)
            {
                return Expression.Lambda<Func<TEntity, bool>>(body, param);
            }
            return null;
        }

        /// <summary>
        /// 生成对应的表达式
        /// </summary>        
        private static Expression CreateQueryExpression(Expression param, object value, string[] propertyPath, QueryCompare compare)
        {
            var member = CreatePropertyExpression(param, propertyPath);
            switch (compare)
            {
                case QueryCompare.Equal:
                    return Equal(member, value);
                case QueryCompare.NotEqual:
                    return NotEqual(member, value);
                case QueryCompare.Like:
                    return Like(member, value);
                case QueryCompare.NotLike:
                    return NotLike(member, value);
                case QueryCompare.StartWidth:
                    return StartsWith(member, value);
                case QueryCompare.EndsWith:
                    return EndsWith(member, value);
                case QueryCompare.LessThan:
                    return LessThan(member, value);
                case QueryCompare.LessThanOrEqual:
                    return LessThanOrEqual(member, value);
                case QueryCompare.GreaterThan:
                    return GreaterThan(member, value);
                case QueryCompare.GreaterThanOrEqual:
                    return GreaterThanOrEqual(member, value);
                //case QueryCompare.Between:
                //    return CreateBetweenExpression(member, value);
                //case QueryCompare.GreaterEqualAndLess:
                //    return GreaterEqualAndLess(member, value);
                //case QueryCompare.Include:
                //    return CreateIncludeExpression(member, value);
                //case QueryCompare.NotInclude:
                //    return CreateNotIncludeExpression(member, value);
                //case QueryCompare.IsNull:
                //    return CreateIsNullExpression(member, value);
                //case QueryCompare.HasFlag:
                //    return CreateHasFlagExpression(member, value);
                default:
                    return null;
            }
        }

        /// <summary>
        /// 生成MemberExpression 
        /// </summary>
        private static MemberExpression CreatePropertyExpression(Expression param, string[] propertyPath)
        {
            return propertyPath.Aggregate(param, Expression.Property) as MemberExpression;
        }

        #region

        /// <summary>
        /// Like
        /// </summary>
        private static Expression Like(MemberExpression member, object value)
        {
            if (member.Type != typeof(string))
            {
                throw new ArgumentOutOfRangeException(nameof(member), $"Member '{member}' can not use 'Like' compare");
            }
            return Expression.Call(member, nameof(string.Contains), null, Expression.Constant(value.ToString()));
        }

        /// <summary>
        /// Not Like
        /// </summary>
        /// <param name="member"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static Expression NotLike(MemberExpression member, object value)
        {
            return Expression.Not(Like(member, value));
        }

        /// <summary>
        /// StartsWith
        /// </summary>
        /// <param name="member"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static Expression StartsWith(MemberExpression member, object value)
        {
            if (member.Type != typeof(string))
            {
                throw new ArgumentOutOfRangeException(nameof(member), $"Member '{member}' can not use 'Like' compare");
            }
            return Expression.Call(member, "StartsWith", null, Expression.Constant(value.ToString()));
        }

        /// <summary>
        /// EndsWith
        /// </summary>
        /// <param name="member"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static Expression EndsWith(MemberExpression member, object value)
        {
            if (member.Type != typeof(string))
            {
                throw new ArgumentOutOfRangeException(nameof(member), $"Member '{member}' can not use 'Like' compare");
            }
            return Expression.Call(member, "EndsWith", null, Expression.Constant(value.ToString()));
        }
      

        /// <summary>
        /// 在条件 AND 仅当第一个操作数的计算结果为才计算第二个操作数的操作 true。 
        /// 它对应于 (a && b) 在 C# 和 (a AndAlso b) 在 Visual Basic 中。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression1"></param>
        /// <param name="expression2"></param>
        /// <returns></returns>
        public static Expression AndAlso(MemberExpression member, object value)
        {
            return Expression.AndAlso(member, Expression.Constant(value, member.Type));
        }

        /// <summary>
        /// 短路条件 OR 操作，如 (a || b) 在 C# 或 (a OrElse b) 在 Visual Basic 中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression1"></param>
        /// <param name="expression2"></param>
        /// <returns></returns>
        public static Expression OrElse(MemberExpression member, object value)
        {
            return Expression.OrElse(member, Expression.Constant(value, member.Type));
        }

        /// <summary>
        /// 一个表示相等比较，如节点 (a == b) 在 C# 或 (a = b) 在 Visual Basic 中。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression1"></param>
        /// <param name="expression2"></param>
        /// <returns></returns>
        public static Expression Equal(MemberExpression member, object value)
        {
            return Expression.Equal(member, Expression.Constant(value, member.Type));
        }

        /// <summary>
        /// "大于"比较，如 (a > b)。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression1"></param>
        /// <param name="expression2"></param>
        /// <returns></returns>
        public static Expression GreaterThan(MemberExpression member, object value)
        {
            return Expression.GreaterThan(member, Expression.Constant(value, member.Type));
        }
        /// <summary>
        /// "大于或等于"比较，如 (a >= b)。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression1"></param>
        /// <param name="expression2"></param>
        /// <returns></returns>
        public static Expression GreaterThanOrEqual(MemberExpression member, object value)
        {
            return Expression.GreaterThanOrEqual(member, Expression.Constant(value));
        }
        /// <summary>
        /// 不相等比较，如 (a != b) 在 C# 或 (a <> b) 在 Visual Basic 中。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression1"></param>
        /// <param name="expression2"></param>
        /// <returns></returns>
        public static Expression NotEqual(MemberExpression member, object value)
        {
            return Expression.NotEqual(member, Expression.Constant(value, member.Type));
        }
        /// <summary>
        /// "小于"比较，如 (a < b)。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression1"></param>
        /// <param name="expression2"></param>
        /// <returns></returns>
        public static Expression LessThan(MemberExpression member, object value)
        {
            return Expression.LessThan(member, Expression.Constant(value, member.Type));
        }
        /// <summary>
        /// "小于或等于"比较，如 (a <= b)。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression1"></param>
        /// <param name="expression2"></param>
        /// <returns></returns>
        public static Expression LessThanOrEqual(MemberExpression member, object value)
        {
            return Expression.LessThanOrEqual(member, Expression.Constant(value, member.Type));
        }
        #endregion


        #region

        /// <summary>
        /// 在条件 AND 仅当第一个操作数的计算结果为才计算第二个操作数的操作 true。 它对应于 (a && b) 在 C# 和 (a AndAlso b) 在 Visual Basic 中。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression1"></param>
        /// <param name="expression2"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2)
        {
            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expression1, expression2));
        }

        /// <summary>
        /// 按位或逻辑 AND 操作，如 (a & b) 在 C# 和 (a And b) 在 Visual Basic 中。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression1"></param>
        /// <param name="expression2"></param>
        /// <returns></returns>
        [Obsolete]
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2)
        {
            return Expression.Lambda<Func<T, bool>>(Expression.And(expression1, expression2));
        }

        /// <summary>
        /// 按位或逻辑 AND 复合赋值运算，如 (a &= b) C# 中。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression1"></param>
        /// <param name="expression2"></param>
        /// <returns></returns>
        [Obsolete]
        public static Expression<Func<T, bool>> AndAssign<T>(this Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2)
        {
            return Expression.Lambda<Func<T, bool>>(Expression.AndAssign(expression1, expression2));
        }

        /// <summary>
        /// 赋值运算，如 (a = b)。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression1"></param>
        /// <param name="expression2"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Assign<T>(this Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2)
        {
            return Expression.Lambda<Func<T, bool>>(Expression.Assign(expression1, expression2));
        }

        /// <summary>
        /// 短路条件 OR 操作，如 (a || b) 在 C# 或 (a OrElse b) 在 Visual Basic 中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression1"></param>
        /// <param name="expression2"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> OrElse<T>(this Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2)
        {
            return Expression.Lambda<Func<T, bool>>(Expression.OrElse(expression1, expression2));
        }

        /// <summary>
        /// 按位或逻辑 OR 复合赋值运算，如 (a |= b) C# 中。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression1"></param>
        /// <param name="expression2"></param>
        /// <returns></returns>
        [Obsolete]
        public static Expression<Func<T, bool>> OrAssign<T>(this Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2)
        {
            return Expression.Lambda<Func<T, bool>>(Expression.OrAssign(expression1, expression2));
        }
        /// <summary>
        /// 按位或逻辑 OR 操作，如 (a | b) 在 C# 或 (a Or b) 在 Visual Basic 中。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression1"></param>
        /// <param name="expression2"></param>
        /// <returns></returns>
        [Obsolete]
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2)
        {
            return Expression.Lambda<Func<T, bool>>(Expression.Or(expression1, expression2));
        }
        /// <summary>
        /// 一个表示相等比较，如节点 (a == b) 在 C# 或 (a = b) 在 Visual Basic 中。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression1"></param>
        /// <param name="expression2"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Equal<T>(this Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2)
        {
            return Expression.Lambda<Func<T, bool>>(Expression.Equal(expression1, expression2));
        }
        /// <summary>
        /// "大于"比较，如 (a > b)。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression1"></param>
        /// <param name="expression2"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GreaterThan<T>(this Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2)
        {
            return Expression.Lambda<Func<T, bool>>(Expression.GreaterThan(expression1, expression2));
        }
        /// <summary>
        /// "大于或等于"比较，如 (a >= b)。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression1"></param>
        /// <param name="expression2"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GreaterThanOrEqual<T>(this Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2)
        {
            return Expression.Lambda<Func<T, bool>>(Expression.GreaterThanOrEqual(expression1, expression2));
        }
        /// <summary>
        /// 不相等比较，如 (a != b) 在 C# 或 (a <> b) 在 Visual Basic 中。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression1"></param>
        /// <param name="expression2"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> NotEqual<T>(this Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2)
        {
            return Expression.Lambda<Func<T, bool>>(Expression.NotEqual(expression1, expression2));
        }
        /// <summary>
        /// "小于"比较，如 (a < b)。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression1"></param>
        /// <param name="expression2"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> LessThan<T>(this Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2)
        {
            return Expression.Lambda<Func<T, bool>>(Expression.LessThan(expression1, expression2));
        }
        /// <summary>
        /// "小于或等于"比较，如 (a <= b)。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression1"></param>
        /// <param name="expression2"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> LessThanOrEqual<T>(this Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2)
        {
            return Expression.Lambda<Func<T, bool>>(Expression.LessThanOrEqual(expression1, expression2));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression1"></param>
        /// <param name="expression2"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> IsTrue<T>(this Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2)
        {
            return Expression.Lambda<Func<T, bool>>(Expression.IsTrue(expression2));
        }
        #endregion
    }
}
