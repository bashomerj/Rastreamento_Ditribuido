using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Bff.Web.DTO;
using Bff.Web.DTO.Contrato;
using Bff.Web.DTO.Proposta;
using Bff.Web.Extensions;
using Core.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bff.Web.Services
{
    public interface ICatalogoService
    {
        Task<ResponseResult> CadastrarProduto(TransferenciaPagamentoPropostaValidacaoDTO transferenciaPagamento);
        Task<ResponseResult> CadastrarServico(TransferenciaPagamentoPropostaValidacaoDTO transferenciaPagamento);


        Task<ResponseResult> Obter(int empresa, int sucursal, int proposta);
    }

    public class CatalogoService : Service, ICatalogoService
    {
        private readonly HttpClient _httpClient;

        public CatalogoService(HttpClient httpClient, IOptions<AppServicesSettings> settings)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(settings.Value.CatalogoUrl);
        }
        public async Task<ResponseResult> CadastrarProduto(TransferenciaPagamentoPropostaValidacaoDTO transferenciaPagamento)
        {
            ResponseResult retorno = new ResponseResult();
            return retorno;
        }
        public async Task<ResponseResult> CadastrarServico(TransferenciaPagamentoPropostaValidacaoDTO transferenciaPagamento)
        {
            ResponseResult retorno = new ResponseResult();
            return retorno;
        }
        public async Task<ResponseResult> Obter(int empresa, int sucursal, int proposta)
        {
            ResponseResult retorno = new ResponseResult();
            return retorno;
        }
    }
   
}



