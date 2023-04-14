using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SEG.Bff.Web.Attributes;
using SEG.Bff.Web.DTO;
using SEG.Bff.Web.DTO.Contrato;
using SEG.Bff.Web.Services;
using SEG.Core.Enum;
using SEG.Core.Messages.Integration;
using SEG.Core.Utils;
using SEG.MessageBus;
using SEG.WebAPI.Core.Controllers;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SEG.Bff.Web.Controllers
{
    [Route("api/v1/contrato")] 
    [ApiController]
    public class ContratoController : MainController
    {
        private readonly ISeguroService _seguroService;

        public ContratoController(ISeguroService seguroService)
        {
            _seguroService = seguroService;

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

            var response = await _seguroService.ObterContratos(produto);
            AdicionarErroProcessamento(response);

            if (response.Status == (int)HttpStatusCode.NotFound)
                return NotFound(response.Errors.Mensagens[0]);

            if (!OperacaoValida()) return CustomResponse();

            return CustomResponse(response.ObterResponseObject<List<ContratoEmpresaDTO>>());

        }

    }
}