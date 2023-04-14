using Microsoft.Extensions.Options;
using ProcessarProposta.Worker.DTOs;
using ProcessarProposta.Worker.Extensions;
using SEG.Core.Communication;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ProcessarProposta.Worker.External_Services
{

    public interface ISeguroService
    {
        Task<ResponseResult> CriarSeguro(SeguroCadastroDTO seguro);
        Task<ResponseResult> CertificadoPorProposta(int empresa, int sucursal, int contrato, int proposta);
    }


    public class SeguroService : Service, ISeguroService
    {
        private readonly HttpClient _httpClient;

        public SeguroService(HttpClient httpClient, IOptions<AppServicesSettings> settings)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(settings.Value.SeguroUrl);
        }

        public async Task<ResponseResult> CriarSeguro(SeguroCadastroDTO seguro)
        {
            ResponseResult retorno = new ResponseResult();
            var seguroContent = ObterConteudo(seguro);

            _httpClient.Timeout = TimeSpan.FromSeconds(120);
            var response = await _httpClient.PostAsync("/api/seguro/criar/certificado", seguroContent);

            if (response.IsSuccessStatusCode)
            {
                var objetoSucesso = await DeserializarObjetoResponse<SeguroCriadoRetornoDTO>(response);
                retorno.AtribuirResponseObject(objetoSucesso);
            }
            else
                retorno = await DeserializarObjetoResponse<ResponseResult>(response);

            return retorno;
        }

        public async Task<ResponseResult> CertificadoPorProposta(int empresa, int sucursal, int contrato, int proposta)
        {
            try
            {
                ResponseResult responseResult = new ResponseResult();
                //var numCrachaContent = ObterConteudo(numCracha);

                _httpClient.Timeout = TimeSpan.FromSeconds(120);
                var response = await _httpClient.GetAsync($"api/seguro/certificado/por-proposta?empresa={empresa}&sucursal={sucursal}&contrato={contrato}&proposta={proposta}");


                if (response.IsSuccessStatusCode)
                {
                    var objetoSucesso = await DeserializarObjetoResponse<CertificadoPorPropostaDTO>(response);
                    responseResult.AtribuirResponseObject(objetoSucesso);
                }else if (response.StatusCode == System.Net.HttpStatusCode.NotFound) 
                {
                    responseResult.Status = (int)response.StatusCode;
                }
                else
                    responseResult = await DeserializarObjetoResponse<ResponseResult>(response);

                return responseResult;
            }
            catch (Exception e)
            {

                throw;
            }

        }
    }
}
