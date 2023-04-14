using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bff.Web.Extensions
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private Stream originalBody;

        public RequestLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<RequestLoggingMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            //var requestId = Guid.NewGuid();

            try
            {
                string tarceId = null;
                if (context.Request.Headers.TryGetValue("traceparent", out StringValues traceParent)) tarceId = traceParent.ToString().Split('-')[1];


                
                context.Request.EnableBuffering();
                await LogRequest(context, tarceId);
                context.Request.Body.Position = 0;
                context.Request.Headers.Add("idRequest", tarceId);




                Stream originalBody = context.Response.Body;
                string responseBody = "";
                try
                {
                    using (var memStream = new MemoryStream())
                    {
                        context.Response.Body = memStream;

                        await _next(context);

                        memStream.Position = 0;
                        responseBody = new StreamReader(memStream).ReadToEnd();

                        memStream.Position = 0;
                        await memStream.CopyToAsync(originalBody);
                    }

                }
                finally
                {
                    context.Response.Body = originalBody;
                    responseBody = responseBody.Replace("\n", "").Replace("\r", "");
                    await LogResponse(context, tarceId, responseBody);
                }

            }
            catch (Exception e)
            {
                throw;
            }
           
        }

        private async Task LogRequest(HttpContext context, string requestId) {
            StreamReader stream = new StreamReader(context.Request?.Body);
            string body = await stream.ReadToEndAsync();
            body = body.Replace("\n", "").Replace("\r", "");

            _logger.LogInformation(
                   "Request {requestId} {method} {url} {queryString} => {data} ",
                   requestId,
                   context.Request?.Method,
                   context.Request?.Path.Value,
                   context.Request?.QueryString,
                   body);

            //TODO: Gravar o request no banco
        }

        private async Task LogResponse(HttpContext context, string requestId, string responseBody) {

            _logger.LogInformation(
                "Response {requestId} {statusCode} => {data} ",
                requestId,
                context.Response?.StatusCode,
                responseBody);
            

            //TODO: Gravar o response no banco
        }
    }
}
