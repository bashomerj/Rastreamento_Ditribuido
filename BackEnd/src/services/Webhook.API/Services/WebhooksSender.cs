using Newtonsoft.Json;
using SEG.Webhook.API.Models.Entities;
using SEG.Webhook.API.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SEG.Webhook.API.Services
{
    public class WebhooksSender : IWebhooksSender
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IWebHookSaidaRepository _webHookSaidaRepository;
        //private readonly ILogger _logger;
        public WebhooksSender(IHttpClientFactory httpClientFactory/*, ILogger<WebhooksSender> logger*/, IWebHookSaidaRepository webHookSaidaRepository)
        {
            _httpClientFactory = httpClientFactory;
            _webHookSaidaRepository = webHookSaidaRepository;
            //_logger = logger;
        }

        public async Task SendAll(IEnumerable<AssinaturaWebhook> receivers, WebhookData data, WebhookType tipo)
        {
            var json = JsonConvert.SerializeObject(data);

            //Descarta a mensagem e grava informando que não tinha nenhum assinante
            if (receivers.Count() == 0)
            {
                var assinatura = new AssinaturaWebhook()
                {
                    id = Guid.NewGuid(),
                    idCliente = Guid.Empty,
                    tipo = tipo.ToString(),
                    dataCriacao = DateTime.Now,
                    urlDestino = null,
                    token = null
                };
                assinatura.id = Guid.Empty;

                await RegistrarSaida(assinatura, json, null);

                await _webHookSaidaRepository.UnitOfWork.Commit();
                await Task.CompletedTask;
            }


            var client = _httpClientFactory.CreateClient();
            var tasks = receivers.Select(r => OnSendData(r, json, client));
            await Task.WhenAll(tasks.ToArray());

            await _webHookSaidaRepository.UnitOfWork.Commit();
            await Task.CompletedTask;

        }

        private async Task OnSendData(AssinaturaWebhook subs, string jsonData, HttpClient client)
        {
                

            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(subs.urlDestino, UriKind.Absolute),
                Method = HttpMethod.Post,
                Content = new StringContent(jsonData, Encoding.UTF8, "application/json")
            };

            if (!string.IsNullOrWhiteSpace(subs.token))
            {
                request.Headers.Add("x-access-token", subs.token);
            }

            var result = await client.SendAsync(request);
            await RegistrarSaida(subs, jsonData, (int)result.StatusCode);
            

            await Task.CompletedTask;
            //return client.SendAsync(request);
        }

        private async Task RegistrarSaida(AssinaturaWebhook subs, string jsonData, int? codigoRetorno)
        {
            await _webHookSaidaRepository.Adicionar(
                    new WebHookSaida()
                    {
                        id = Guid.NewGuid(),
                        idAssinatura = subs.id,
                        tipo = subs.tipo,
                        dataEnvio = DateTime.Now,
                        urlDestino = subs.urlDestino,
                        token = subs.token,
                        payload = jsonData,
                        codigoRetorno = codigoRetorno
                    });
        }
    }
}
