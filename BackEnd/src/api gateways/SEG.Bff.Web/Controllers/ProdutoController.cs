using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SEG.Bff.Web.Attributes;
using SEG.Bff.Web.DTO;
using SEG.Bff.Web.DTO.Contrato;
using SEG.Bff.Web.DTO.Produto;
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
    [Route("api/v1/produto")]
    [ApiController]
    public class ProdutoController : MainController
    {
        private readonly IProdutosService _produtoService;
        

        public ProdutoController(IProdutosService produtoService)
        {
            _produtoService = produtoService;

        }


        [SwaggerOperation(summary: "Utilizado para obter todas as coberturas oferecidas no produto",
           description: "Através do produto são retornadas todas as coberturas comercializadas")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponse(statusCode: 200, type: typeof(CoberturaPorProdutoDTO), description: "Retorna a lista de coberturas")]
        [SwaggerResponse(statusCode: 400, type: typeof(ValidationProblemDetails), description: "Parâmetros inválidos")]
        [SwaggerResponse(statusCode: 404, type: typeof(string), description: "Coberturas não encontradas")]
        [HttpGet]
        [Route("coberturas")]
        public async Task<IActionResult> ObterCoberturas(
           [SwaggerParameter(Required = true)]
            int produto)
        {

            var response = await _produtoService.ObterCoberturas(produto);
            AdicionarErroProcessamento(response);

            if (response.Status == (int)HttpStatusCode.NotFound)
                return NotFound(response.Errors.Mensagens[0]);

            if (!OperacaoValida()) return CustomResponse();

            return CustomResponse(response.ObterResponseObject<List<CoberturaPorProdutoDTO>>());
        }



    }
}