using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SEG.Bff.Web.DTO;
using SEG.Bff.Web.Extensions;
using SEG.Bff.Web.Models;
using SEG.Core.Communication;


namespace SEG.Bff.Web.Services
{
    public interface IPessoaService
    {
        Task<ResponseResult> ValidarColaborador(int numCracha);
        Task<ResponseResult> CadastrarListaPessoaSegurado(List<CadastrarPessoaSeguradoDTO> cadastrarPessoa);
        Task<ResponseResult> VerificarCpfExistente(VerificarDocumentoExistente cpf);
        Task<ResponseResult> VerificarRgExistente(VerificarDocumentoExistente rg);
        Task<ResponseResult> ValidarInclusaoPessoa(List<CadastrarPessoaSeguradoDTO> listaSeguradosValidacao);
        Task<ResponseResult> ValidarInclusaoListaBeneficiario(List<BeneficiarioDTO> listaBeneficiarios);
        //Task<ResponseResult> ObterPessoaSerasa(string cpf);



    }

    public class PessoaService : Service, IPessoaService
    {
        private readonly HttpClient _httpClient;

        public PessoaService(HttpClient httpClient, IOptions<AppServicesSettings> settings)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(settings.Value.PessoaUrl);
        }

        public async Task<ResponseResult> ValidarColaborador(int numCracha)
        {
            //var numCrachaContent = ObterConteudo(numCracha);

            var response = await _httpClient.GetAsync($"api/pessoa/colaborador/validar/{numCracha}");

            if (!TratarErrosResponse(response)) return await DeserializarObjetoResponse<ResponseResult>(response);

            return RetornoOk();
        }

        public async Task<ResponseResult> CadastrarListaPessoaSegurado(List<CadastrarPessoaSeguradoDTO> cadastrarPessoa)
        {
            ResponseResult retorno = new ResponseResult();
            var cadastrarPessoaContent = ObterConteudo(cadastrarPessoa);

            var response = await _httpClient.PostAsync("api/pessoa/cadastrar/lista-segurado", cadastrarPessoaContent);

            //var retornoApi = await DeserializarObjetoResponse<List<CadastrarPessoaSeguradoDTO>>(response);
            if (response.IsSuccessStatusCode)
           {
                var objetoSucesso = await DeserializarObjetoResponse<List<CadastrarPessoaSeguradoDTO>>(response);
                retorno.AtribuirResponseObject(objetoSucesso);
            }
            else
                retorno = await DeserializarObjetoResponse<ResponseResult>(response);
            

            return retorno;
        }

        public async Task<ResponseResult> VerificarCpfExistente(VerificarDocumentoExistente cpf)
        {
            var cpfContent = ObterConteudo(cpf);

            var response = await _httpClient.PostAsync("api/pessoa/verificar/cpf/existente", cpfContent);

            if (!TratarErrosResponse(response)) return await DeserializarObjetoResponse<ResponseResult>(response);

            return RetornoOk();
        }

        public async Task<ResponseResult> VerificarRgExistente(VerificarDocumentoExistente rg)
        {
            var rgContent = ObterConteudo(rg);

            var response = await _httpClient.PostAsync("api/pessoa/verificar/rg/existente", rgContent);

            if (!TratarErrosResponse(response)) return await DeserializarObjetoResponse<ResponseResult>(response);

            return RetornoOk();
        }

        public async Task<ResponseResult> ValidarInclusaoPessoa(List<CadastrarPessoaSeguradoDTO> listaSeguradosValidacao)
        {
            var listaSeguradosValidacaoContent = ObterConteudo(listaSeguradosValidacao);

            var response = await _httpClient.PostAsync("api/pessoa/validar/inclusao-lista-segurado", listaSeguradosValidacaoContent);

            if (!TratarErrosResponse(response)) return await DeserializarObjetoResponse<ResponseResult>(response);

            return RetornoOk();
        }

        public async Task<ResponseResult> ValidarInclusaoListaBeneficiario(List<BeneficiarioDTO> listaBeneficiarios)
        {
            var listaBeneficiariosContent = ObterConteudo(listaBeneficiarios);

            var response = await _httpClient.PostAsync("api/pessoa/validar/inclusao-lista-beneficiario", listaBeneficiariosContent);

            if (!TratarErrosResponse(response)) return await DeserializarObjetoResponse<ResponseResult>(response);

            return RetornoOk();
        }

        //public async Task<ResponseResult> ObterPessoaSerasa(string cpf)
        //{

        //    var response = await _httpClient.GetAsync($"api/pessoa/serasa{cpf}");

        //    if (!TratarErrosResponse(response)) return await DeserializarObjetoResponse<ResponseResult>(response);

        //    return RetornoOk();
        //}

    }
}