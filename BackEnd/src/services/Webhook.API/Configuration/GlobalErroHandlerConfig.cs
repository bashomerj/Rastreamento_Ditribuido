using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Diagnostics;

namespace SEG.Webhook.API.Configuration
{
    public static class GlobalErroHandlerConfig
    {
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

						var problemDetails = new ProblemDetails
						{
							Instance = context.Request.HttpContext.Request.Path
						};

						//var problemDetails = new ValidationProblemDetails(new ModelStateDictionary()); 

						if (exception is BadHttpRequestException badHttpRequestException)
						{
							problemDetails.Title = "The request is invalid";
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

						var logger = loggerFactory.CreateLogger("GlobalExceptionHandler");
						logger.LogError($"Erro Inesperado: {JsonConvert.SerializeObject(problemDetails)}");

						problemDetails.Detail = "Ocorreu um erro interno na aplicação que impossibilitou o processamento por completo da requisição. Favor tentar mais tarde.";

						await context.Response.WriteAsync(JsonConvert.SerializeObject(problemDetails));
					}
				});
			});
		}

		public static IServiceCollection ConfigureGlobalErroHandler(this IServiceCollection services)
		{
			return services.Configure<ApiBehaviorOptions>(options =>
			{
				options.InvalidModelStateResponseFactory = context =>
				{
					var problemDetails = new ValidationProblemDetails(context.ModelState)
					{
						Instance = context.HttpContext.Request.Path,
						Status = StatusCodes.Status400BadRequest,
						Detail = "Please refer to the errors property for additional details"
					};


					return new BadRequestObjectResult(problemDetails)
					{
						ContentTypes = { "application/problem+json", "application/problem+xml" }
					};
				};
			});
		}

	}

	//public static class DetalheErroExtensions
	//{
	//	public static void UseDetalheErroExceptionHandler(this IApplicationBuilder app, ILoggerFactory loggerFactory)
	//	{
	//		app.UseExceptionHandler(builder =>
	//		{
	//			builder.Run(async context =>
	//			{
	//				var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();

	//				if (exceptionHandlerFeature != null)
	//				{
	//					var exception = exceptionHandlerFeature.Error;

	//					var problemDetails = new ProblemDetails
	//					{
	//						Instance = context.Request.HttpContext.Request.Path
	//					};

	//					//var problemDetails = new ValidationProblemDetails(new ModelStateDictionary()); 

	//					if (exception is BadHttpRequestException badHttpRequestException)
	//					{
	//						problemDetails.Title = "The request is invalid";
	//						problemDetails.Status = StatusCodes.Status400BadRequest;
	//						problemDetails.Detail = badHttpRequestException.Message;
	//					}
	//					else
	//					{
	//						problemDetails.Title = exception.Message;
	//						problemDetails.Status = StatusCodes.Status500InternalServerError;
	//						problemDetails.Detail = exception.Demystify().ToString();
	//					}

	//					context.Response.StatusCode = problemDetails.Status.Value;
	//					context.Response.ContentType = "application/problem+json";

	//					var logger = loggerFactory.CreateLogger("GlobalExceptionHandler");
	//					logger.LogError($"Erro Inesperado: {JsonConvert.SerializeObject(problemDetails)}");

	//					problemDetails.Detail = "Ocorreu um erro interno na aplicação que impossibilitou o processamento por completo da requisição. Favor tentar mais tarde.";

	//					await context.Response.WriteAsync(JsonConvert.SerializeObject(problemDetails));
	//				}
	//			});
	//		});
	//	}

	//	public static IServiceCollection ConfigureDetalheErroModelState(this IServiceCollection services)
	//	{
	//		return services.Configure<ApiBehaviorOptions>(options =>
	//		{
	//			options.InvalidModelStateResponseFactory = context =>
	//			{
	//				var problemDetails = new ValidationProblemDetails(context.ModelState)
	//				{
	//					Instance = context.HttpContext.Request.Path,
	//					Status = StatusCodes.Status400BadRequest,
	//					Detail = "Please refer to the errors property for additional details"
	//				};


	//				return new BadRequestObjectResult(problemDetails)
	//				{
	//					ContentTypes = { "application/problem+json", "application/problem+xml" }
	//				};
	//			};
	//		});
	//	}
	//}

	public static class ProblemDetailsExtensions
	{

	}
}
