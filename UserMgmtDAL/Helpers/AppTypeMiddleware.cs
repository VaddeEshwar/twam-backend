using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserMgmtDAL.Helpers
{
    public class AppTypeMiddleware
    {
        private readonly RequestDelegate _next;

        public AppTypeMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Assume the app type is passed as a header
            var appType = context.Request.Headers["AppType"].ToString();

            if (string.IsNullOrEmpty(appType))
            {
                appType = "Type1"; // Default app type
            }

            context.Items["AppType"] = appType;

            await _next(context);
        }
    }
}
