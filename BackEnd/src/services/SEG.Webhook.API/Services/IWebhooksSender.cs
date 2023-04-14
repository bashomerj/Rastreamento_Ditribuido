using SEG.Webhook.API.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SEG.Webhook.API.Services
{
    public interface IWebhooksSender
    {
        Task SendAll(IEnumerable<AssinaturaWebhook> receivers, WebhookData data, WebhookType tipo);
    }
}
