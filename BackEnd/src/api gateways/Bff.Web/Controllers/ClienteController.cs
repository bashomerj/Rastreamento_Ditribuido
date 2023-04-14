using Bff.Web.DTO.Contrato;
using Bff.Web.Services;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Core.Controllers;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using System;

namespace Bff.Web.Controllers
{
    [Route("api/v1/cliente")] 
    [ApiController]
    public class ClienteController : MainController
    {
        private readonly IClienteService _clienteService;

        public ClienteController(IClienteService clienteService)
        {
            _clienteService = clienteService;

        }

        [SwaggerOperation(summary: "Utilizado para obter todos os contratos, empresas e sucursais relacionados a um produto", 
            description: "Através do produto são retornado todos os contratos comercializados e suas respectivas empresas e sucursais")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponse(statusCode: 200, type: typeof(ContratoEmpresaDTO), description: "Retorna os contratos")]
        [SwaggerResponse(statusCode: 400, type: typeof(ValidationProblemDetails), description: "Parâmetros inválidos")]
        [SwaggerResponse(statusCode: 404, type: typeof(string), description: "Contrato não encontrado")]
        [HttpGet]
        [Route("contratos")]
        public async Task<IActionResult> ObterContratos(
            [SwaggerParameter(Required = true)]
            int produto)
        {
            return new Random().Next(0, 6) switch
            {
                //0 => throw new Exception("Aconteceu um erro inesperado no sistema. Sua solicitação não foi processada por um erro interno"),
                //1 => throw new ApplicationException("Erro na aplicação"),
                _ => CustomResponse("Processado com sucesso!")
            } ;

            
        }

    }
}