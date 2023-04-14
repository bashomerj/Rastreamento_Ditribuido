using Microsoft.Extensions.Options;
using Bff.Web.DTO;
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
    public interface IClienteService
    {
        Task<ResponseResult> Cadastrar(GerarParcelaDTO gerarParcelaDTO);
        Task<ResponseResult> Alterar(GerarParcelaDTO gerarParcelaDTO);
        Task<ResponseResult> Obter(GerarParcelaDTO gerarParcelaDTO);
        Task<ResponseResult> Remover(GerarParcelaDTO gerarParcelaDTO);
    }

    public class ClienteService : Service, IClienteService
    {
        private readonly HttpClient _httpClient;

        public ClienteService(HttpClient httpClient, IOptions<AppServicesSettings> settings)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(settings.Value.ClienteUrl);
        }

     


        public async Task<ResponseResult> Cadastrar(GerarParcelaDTO gerarParcelaDTO )
        {
            return RetornoOk();
        }

        public async Task<ResponseResult> Alterar(GerarParcelaDTO gerarParcelaDTO)
        {
            return RetornoOk();
        }

        public async Task<ResponseResult> Obter(GerarParcelaDTO gerarParcelaDTO)
        {
            return RetornoOk();
        }

        public async Task<ResponseResult> Remover(GerarParcelaDTO gerarParcelaDTO)
        {
            return RetornoOk();
        }
    }
   
}



