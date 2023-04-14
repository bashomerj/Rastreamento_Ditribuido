using Microsoft.Extensions.Options;
using ProcessarProposta.Worker.DTOs;
using ProcessarProposta.Worker.Extensions;
using Core.Communication;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ProcessarProposta.Worker.External_Services
{

    public interface IPessoaService
    {
        Task<ResponseResult> CadastrarListaPessoaSegurado(List<SeguradoCadastroDTO> cadastrarPessoa);
    }


    public class PessoaService : Service, IPessoaService
    {
        private readonly HttpClient _httpClient;

        public PessoaService(HttpClient httpClient, IOptions<AppServicesSettings> settings)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(settings.Value.PessoaUrl);
        }

        public async Task<ResponseResult> CadastrarListaPessoaSegurado(List<SeguradoCadastroDTO> segurados)
        {
            ResponseResult retorno = new ResponseResult();
            var seguradosContent = ObterConteudo(segurados);

            _httpClient.Timeout = TimeSpan.FromSeconds(120);
            var response = await _httpClient.PostAsync("api/pessoa/cadastrar/lista-segurado", seguradosContent);

            if (response.IsSuccessStatusCode)
            {
                var objetoSucesso = await DeserializarObjetoResponse<List<SeguradoCadastroDTO>>(response);
                retorno.AtribuirResponseObject(objetoSucesso);
            }
            else
                retorno = await DeserializarObjetoResponse<ResponseResult>(response);

            return retorno;
        }
    }
}
