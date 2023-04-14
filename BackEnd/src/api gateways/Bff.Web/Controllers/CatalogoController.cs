using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Bff.Web.Attributes;
using Bff.Web.DTO;
using Bff.Web.DTO.Proposta;
using Bff.Web.Services;
using Core.Enum;
using Core.Messages.Integration;
using Core.Utils;
using MessageBus;
using WebAPI.Core.Controllers;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Bff.Web.Controllers
{
    [Route("api/v1/catalogo")]
    [ApiController]
    public class CatalogoController : MainController
    {
        private readonly ICatalogoService _catalogoService;
        private readonly IMapper _mapper;
        private readonly IMessageBus _bus;

        public CatalogoController(ICatalogoService catalogoService, IMapper mapper, IMessageBus bus)
        {
            _catalogoService = catalogoService;
            _mapper = mapper;
            _bus = bus;
        }



        [SwaggerOperation(summary: "Utilizado para transmitir uma proposta para processamento na seguradora", description: "Utilize essa função para enviar propostas para o processo de aceitação do seguro. As propostas enviadas são validadas, e quando aceitas, colocadas na fila de processamento para execução do processo de aceitação do seguro. Esse processo de aceitação consiste em passar por várias análise que são feitas pela seguradora para identificar se o risco da proposta está dentro dos parametros aceitos pela seguradora.")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponse(statusCode: 200, type: typeof(string), description: "Retorna o aceite da requisição para processamento da proposta. Proposta adicionada na fila de processamento. Após o processamento da mensagem na fila a reposta será enviada via webhook")]
        [SwaggerResponse(statusCode: 400, type: typeof(ValidationProblemDetails), description: "Parâmetros inválidos")]
        [HttpPost]
        [Route("cadastrar")]
        public async Task<IActionResult> Cadastrar(NovaPropostaDTO novaPropostaDTO)
        {
            return CustomResponse();
        }

    }
}