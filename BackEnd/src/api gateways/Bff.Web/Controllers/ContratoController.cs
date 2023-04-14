using Bff.Web.DTO.Contrato;
using Bff.Web.Services;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Core.Controllers;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;

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

            var response = await _clienteService.Obter(new DTO.GerarParcelaDTO());
            AdicionarErroProcessamento(response);

            if (response.Status == (int)HttpStatusCode.NotFound)
                return NotFound(response.Errors.Mensagens[0]);

            if (!OperacaoValida()) return CustomResponse();

            return CustomResponse(response.ObterResponseObject<List<ContratoEmpresaDTO>>());

        }

    }
}