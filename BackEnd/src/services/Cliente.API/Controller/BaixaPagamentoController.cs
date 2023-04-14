using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Cliente.API.DTO;
using Cliente.API.Services;
using SEG.Core.Mediator;
using SEG.WebAPI.Core.Controllers;
using System.Threading.Tasks;

namespace Cliente.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaixaPagamentoController : MainController
    {

        private readonly IMediatorHandler _mediator;
        private readonly IBaixaPagamentoService _baixaPagamentoService;


        public BaixaPagamentoController(IMediatorHandler mediator, IBaixaPagamentoService baixaPagamentoService)
        {
            _mediator = mediator;
            _baixaPagamentoService = baixaPagamentoService;
        }

        
        
        [HttpPost]
        [Route("transferencia-pagamento")]
        public async Task<IActionResult> TransferenciaPagamento(TransferenciaPagamentoDTO  transferenciaPagamentoDTO)
        {
            
                ValidationResult sucesso;

                sucesso = await _baixaPagamentoService.TransferenciaPagamentoProposta(
                    transferenciaPagamentoDTO.empresa,
                    transferenciaPagamentoDTO.sucursal,
                    transferenciaPagamentoDTO.usuario,
                    transferenciaPagamentoDTO.nossoNumeroOrigem,
                    transferenciaPagamentoDTO.nossoNumeroDestino);

                return CustomResponse(sucesso);

        }


    }
}
