using Microsoft.Extensions.Options;
using SEG.Bff.Web.DTO;
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
    public interface ICobrancaService
    {
        Task<ResponseResult> GerarParcela(GerarParcelaDTO gerarParcelaDTO);
    }

    public class CobrancaService : Service, ICobrancaService
    {
        private readonly HttpClient _httpClient;

        public CobrancaService(HttpClient httpClient, IOptions<AppServicesSettings> settings)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(settings.Value.CobrancaUrl);
        }

     


        public async Task<ResponseResult> GerarParcela(GerarParcelaDTO gerarParcelaDTO )
        {
            var gerarParcelaContent = ObterConteudo(gerarParcelaDTO);

            var response = await _httpClient.PostAsync("api/ParcelaPremio", gerarParcelaContent);

            if (!TratarErrosResponse(response)) return await DeserializarObjetoResponse<ResponseResult>(response);

            return RetornoOk();
        }
    }
   
}



