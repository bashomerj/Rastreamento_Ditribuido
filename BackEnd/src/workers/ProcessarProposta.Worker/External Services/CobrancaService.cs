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

    public interface ICobrancaService
    {
        Task<ResponseResult> GerarParcela(GerarParcelaDTO parcela);
    }


    public class CobrancaService : Service, ICobrancaService
    {
        private readonly HttpClient _httpClient;

        public CobrancaService(HttpClient httpClient, IOptions<AppServicesSettings> settings)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(settings.Value.CobrancaUrl);
        }

        public async Task<ResponseResult> GerarParcela(GerarParcelaDTO gerarParcelaDTO)
        {
            var gerarParcelaContent = ObterConteudo(gerarParcelaDTO);

            _httpClient.Timeout = TimeSpan.FromSeconds(120);
            var response = await _httpClient.PostAsync("api/ParcelaPremio", gerarParcelaContent);

            if (!TratarErrosResponse(response)) return await DeserializarObjetoResponse<ResponseResult>(response);

            return RetornoOk();
        }
    }
}
