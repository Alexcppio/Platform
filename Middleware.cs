using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Platform
{
    public class QueryStringMiddleWare
    {
        private RequestDelegate next;
        public QueryStringMiddleWare()
        {
            // Ничего не делать
        }
        public QueryStringMiddleWare(RequestDelegate nextDelegate)
        {
            next = nextDelegate;
        }
        public async Task Invoke(HttpContext context)
        {
            if(context.Request.Method == HttpMethods.Get
                && context.Request.Query["custom"] == "true")
            {
                await context.Response.WriteAsync("Class-based Middleware\n");
            }
            if(next != null)
            {
                await next(context);
            }
        }
    }
}
