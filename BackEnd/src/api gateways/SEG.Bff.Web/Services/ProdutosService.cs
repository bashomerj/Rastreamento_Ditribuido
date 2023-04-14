using Microsoft.Extensions.Options;
using SEG.Bff.Web.DTO;
using SEG.Bff.Web.DTO.Contrato;
using SEG.Bff.Web.DTO.Produto;
using SEG.Bff.Web.Extensions;
using SEG.Core.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SEG.Bff.Web.Services
{
    public interface IProdutosService
    {
        Task<ResponseResult> VerificarSeListaCoberturaExiste(List<short> listaCobertura);
        Task<ResponseResult> ObterCoberturas(int produto);
    }


    public class ProdutosService : Service, IProdutosService
    {
        private readonly HttpClient _httpClient;

        public ProdutosService(HttpClient httpClient, IOptions<AppServicesSettings> settings)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(settings.Value.ProdutosUrl);
        }

        public async Task<ResponseResult> VerificarSeListaCoberturaExiste(List<short> listaCobertura)
        {
            var listaCoberturaContent = ObterConteudo(listaCobertura);

            var response = await _httpClient.PostAsync("/api/produtos/verificar/lista-cobertura/existente", listaCoberturaContent);

            if (!TratarErrosResponse(response)) return await DeserializarObjetoResponse<ResponseResult>(response);

            return RetornoOk();
        }

        public async Task<ResponseResult> ObterCoberturas(int produto)
        {
            ResponseResult retorno = new ResponseResult();

            var response = await _httpClient.GetAsync($"api/produtos/coberturas?produto={produto}");
            if (!TratarErrosResponse(response)) return await DeserializarObjetoResponse<ResponseResult>(response);

            retorno.Status = (int)response.StatusCode;
            if (response.IsSuccessStatusCode)
            {
                var objetoSucesso = await DeserializarObjetoResponse<List<CoberturaPorProdutoDTO>>(response);
                retorno.AtribuirResponseObject<List<CoberturaPorProdutoDTO>>(objetoSucesso);
            }
            //else if (!(response.StatusCode == HttpStatusCode.NotFound))
            //    retorno = await DeserializarObjetoResponse<ResponseResult>(response);


            return retorno;
        }
    }
}
