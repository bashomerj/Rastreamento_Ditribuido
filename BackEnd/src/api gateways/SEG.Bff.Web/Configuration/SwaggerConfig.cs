using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SEG.Bff.Web.Attributes;

namespace SEG.Bff.Web.Configuration.Configuration
{
    public static class SwaggerConfig
    {
        public static void AddSwaggerConfiguration(this IServiceCollection services, IWebHostEnvironment env)
        {
            //Serve para o enum sair com o texto em vez do código
            services.AddSwaggerGenNewtonsoftSupport();
            //Serve para sair a definição descritiva dos campos que são enum
            services.AddSwaggerGen(SwaggerGenOptionsExtensions.UseInlineDefinitionsForEnums);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Title = "Sinaf Seguros - BFF API Gateway",
                    Description = "API utilizada para fazer a camada de API Gateway BFF (Backend for Frontend)",
                    Contact = new OpenApiContact() { Name = "Fatima Regina", Email = "fatima@sinaf.com.br" }
                    /*License = new OpenApiLicense() { Name = "MIT", Url = new Uri("https://opensource.org/licenses/MIT") }*/
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Insira o token JWT desta maneira: Bearer {seu token}",
                    Name = "Authorization",
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}
                        }
                    });

                //Adicionando o filtro de schema de exemplo a configuração
                c.SchemaFilter<SwaggerSchemaExampleFilter>();
                c.SchemaFilter<SwaggerSchemaMinimumAndMaximumFilter>();
                //Habilitando as anotações
                c.EnableAnnotations();
                //Inclusão do arquivo XML do swagger
                string path = $"{AppContext.BaseDirectory}{Path.DirectorySeparatorChar}{env.ApplicationName}.xml";
                c.IncludeXmlComments(path);
                //Adicionado o filtro para traduzir enum de int para string
                //c.SchemaFilter<EnumTypesSchemaFilter>(path);
            });

        }

        public static void UseSwaggerConfiguration(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                //Remove a apresentação dos schemas
                c.DefaultModelsExpandDepth(-1);
            });
        }
    }
}