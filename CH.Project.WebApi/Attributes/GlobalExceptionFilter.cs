using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CH.Project.WebApi.Attributes
{
    public class GlobalExceptionFilter : Attribute,IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            context.Result = new JsonResult(new { code = 500, err = "系统异常" });
            context.ExceptionHandled = true;
        }
    }
}
