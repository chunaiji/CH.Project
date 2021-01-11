//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Reflection;
//using System.Text;

//namespace CH.Project.Commont.QueryCommont
//{
//    public static class GenerateQueryExpression
//    {
//        public static Expression<Func<TEntity, bool>> GenerateQueryExpressionCore<TEntity>(this IQueryModule<TEntity> queryModule) where TEntity : class
//        {
//            if (queryModule == null)
//            {
//                return null;
//            }
//            var type = queryModule.GetType();
//            var param = Expression.Parameter(typeof(TEntity), "m");
//            Expression body = null;
//            foreach (PropertyInfo property in type.GetProperties())
//            {
//                var value = property.GetValue(queryModule);
//                if (value is string)
//                {
//                    value = string.IsNullOrEmpty((string)value) ? string.Empty : value.ToString().Trim();
//                }
//                Expression sub = null;
//                foreach (var attribute in property.GetCustomAttributes(typeof(QueryModeAttribute), true))
//                {
//                    var propertyPath = ((Attribute)attribute).PropertyPath;
//                    if (propertyPath == null || propertyPath.Length == 0)
//                    {
//                        propertyPath = new[] { property.Name };
//                    }

//                    var experssion = CreateQueryExpression(param, value, propertyPath, ((Attribute)attribute).Compare);
//                    if (experssion != null)
//                    {
//                        sub = sub == null ? experssion : Expression.Or(sub, experssion);
//                    }
//                }
//                if (sub != null)
//                {
//                    body = body == null ? sub : Expression.And(body, sub);
//                }
//            }
//            if (body != null)
//            {
//                return Expression.Lambda<Func<TEntity, bool>>(body, param);
//            }

//            return null;



//        }

//        /// <summary>
//        /// 生成对应的表达式
//        /// </summary>        
//        private static Expression CreateQueryExpression(Expression param, object value, string[] propertyPath, QueryCompare compare)
//        {
//            var member = CreatePropertyExpression(param, propertyPath);

//            switch (compare)
//            {
//                case QueryCompare.Equal:
//                    return CreateEqualExpression(member, value);
//                //case QueryCompare.NotEqual:
//                //    return CreateNotEqualExpression(member, value);
//                //case QueryCompare.Like:
//                //    return CreateLikeExpression(member, value);
//                //case QueryCompare.NotLike:
//                //    return CreateNotLikeExpression(member, value);
//                //case QueryCompare.StartWidth:
//                //    return CreateStartsWithExpression(member, value);
//                //case QueryCompare.LessThan:
//                //    return CreateLessThanExpression(member, value);
//                //case QueryCompare.LessThanOrEqual:
//                //    return CreateLessThanOrEqualExpression(member, value);
//                //case QueryCompare.GreaterThan:
//                //    return CreateGreaterThanExpression(member, value);
//                //case QueryCompare.GreaterThanOrEqual:
//                //    return CreateGreaterThanOrEqualExpression(member, value);
//                //case QueryCompare.Between:
//                //    return CreateBetweenExpression(member, value);
//                //case QueryCompare.GreaterEqualAndLess:
//                //    return CreateGreaterEqualAndLessExpression(member, value);
//                //case QueryCompare.Include:
//                //    return CreateIncludeExpression(member, value);
//                //case QueryCompare.NotInclude:
//                //    return CreateNotIncludeExpression(member, value);
//                //case QueryCompare.IsNull:
//                //    return CreateIsNullExpression(member, value);
//                //case QueryCompare.HasFlag:
//                //    return CreateHasFlagExpression(member, value);
//                default:
//                    return null;
//            }
//        }

//        /// <summary>
//        /// 生成MemberExpression 
//        /// </summary>
//        private static MemberExpression CreatePropertyExpression(Expression param, string[] propertyPath)
//        {
//            var expression = propertyPath.Aggregate(param, Expression.Property) as MemberExpression;
//            return expression;
//        }

//        /// <summary>
//        /// 生成等于的表达式
//        /// </summary>
//        private static Expression CreateEqualExpression(MemberExpression member, object value)
//        {
//            if (value == null)
//            {
//                return null;
//            }
//            var val = Expression.Constant(ChangeType(value, member.Type), member.Type);
//            return Expression.Equal(member, val);
//        }

//        /// <summary>
//        /// 生成Sql中的like（contain）表达式
//        /// </summary>
//        private static Expression CreateLikeExpression(MemberExpression member, object value)
//        {
//            if (value == null)
//            {
//                return null;
//            }

//            if (member.Type != typeof(string))
//            {
//                throw new ArgumentOutOfRangeException(nameof(member), $"Member '{member}' can not use 'Like' compare");
//            }

//            var str = value.ToString();
//            var val = Expression.Constant(str);

//            return Expression.Call(member, nameof(string.Contains), null, val);
//        }
//    }
//}
