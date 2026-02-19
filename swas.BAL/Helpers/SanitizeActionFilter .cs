using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace swas.BAL.Helpers
{
    public class SanitizeActionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            foreach (var arg in context.ActionArguments.Values)
            {
                if (arg == null) continue;
                foreach (var prop in arg.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (prop.CanRead && prop.CanWrite && prop.PropertyType == typeof(string))
                    {
                        var val = (string)prop.GetValue(arg);
                        if (!string.IsNullOrEmpty(val))
                        {
                            prop.SetValue(arg, WebUtility.HtmlEncode(val));
                        }
                    }
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
