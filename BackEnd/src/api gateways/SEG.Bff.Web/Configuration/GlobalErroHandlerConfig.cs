using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Server.IIS;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace SEG.Bff.Web.Configuration
{
    public static class GlobalErroHandlerConfig
    {
		//usa e configura o middle de erros do .net  
		public static void UseGlobalErroHandler(this IApplicationBuilder app, ILoggerFactory loggerFactory)
		{
			app.UseExceptionHandler(builder =>
			{
				builder.Run(async context =>
				{
					var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();

					if (exceptionHandlerFeature != null)
					{
						var exception = exceptionHandlerFeature.Error;

						var problemDetails = new ValidationProblemDetails
						{
							Instance = context.Request.HttpContext.Request.Path
						};

						//var problemDetails = new ValidationProblemDetails(new ModelStateDictionary()); 


						//if (exception is System.DivideByZeroException divideByZeroException)
						//{
						//	problemDetails.Title = exception.Message;
						//	problemDetails.Status = StatusCodes.Status500InternalServerError;
						//	problemDetails.Detail = exception.Demystify().ToString();
						//}
						if (exception is BadHttpRequestException badHttpRequestException)
						{
							problemDetails.Title = "Requisição falhou";
							problemDetails.Status = StatusCodes.Status400BadRequest;
							problemDetails.Detail = badHttpRequestException.Message;
						}
						else
						{
							problemDetails.Title = exception.Message;
							problemDetails.Status = StatusCodes.Status500InternalServerError;
							problemDetails.Detail = exception.Demystify().ToString();
						}

						context.Response.StatusCode = problemDetails.Status.Value;
						context.Response.ContentType = "application/problem+json";

						var tracertId = GetTraceId(context);
						if (string.IsNullOrEmpty(tracertId)) tracertId = "null";

						var logger = loggerFactory.CreateLogger(nameof(GlobalErroHandlerConfig));
						logger.LogError($"TracertID: {tracertId} ; Mensagem: {JsonConvert.SerializeObject(problemDetails)}");

						problemDetails.Detail = "Ocorreu um erro interno na aplicação que impossibilitou o processamento por completo da requisição. Favor tentar mais tarde.";

						await context.Response.WriteAsync(JsonConvert.SerializeObject(problemDetails));
					}
				});
			});
		}

		private static string GetTraceId(HttpContext httpContext)
		{
			if (httpContext == null)
			{
				throw new ArgumentNullException(nameof(httpContext));
			}

			if (httpContext.Request.Headers.TryGetValue("traceparent", out StringValues traceParent))
			{
				var tracertParentArguments = traceParent.ToString().Split('-');

				//traceparent é definido pelo padrão W3C Trace Context
				//00: indica a versão do formato de cabeçalho;
				//< trace - id >: um identificador único de 16 bytes(32 caracteres hexadecimais) que identifica a cadeia de rastreamento(Trace);
				//< parent - id >: um identificador único de 8 bytes(16 caracteres hexadecimais) que identifica a solicitação pai(Parent);
				//< trace - flags >: um campo de 1 byte que contém informações sobre o rastreamento(Trace Flags).
				if (tracertParentArguments.Length == 4) return tracertParentArguments[1];

				//se passado somente o tracertID
				if (tracertParentArguments.Length == 1) return tracertParentArguments[0];
			}

			return null;
		}

		public static string GenerateTraceParentW3C()
		{
			var traceId = Guid.NewGuid().ToString("N");
			var parentId = Guid.NewGuid().ToString("N").Substring(0, 16);
			var traceFlags = "01"; // Use "00" para desativar o rastreamento

			return $"00-{traceId}-{parentId}-{traceFlags}";
		}

		//configura os erros do model binding

		public static void ConfigureGlobalErroHandler(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Instance = context.HttpContext.Request.Path,
                        Status = StatusCodes.Status400BadRequest,
                        Detail = "Dados da requisição inválidos",
                    };

                    return new BadRequestObjectResult(problemDetails)
                    {
                        ContentTypes = { "application/problem+json", "application/problem+xml" }
                    };
                };
            });
        }

    }
}
