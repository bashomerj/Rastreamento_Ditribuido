using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SEG.Webhook.API.Models.Entities
{
    public class WebhookData
    {
        public Guid Id { get; set; }
        public DateTime When { get; set; }
        public string Payload { get; set; }
        public string Type { get; set; }


        public WebhookData(WebhookType hookType, object data)
        {
            //When = DateTime.UtcNow;
            Id = Guid.NewGuid();
            When = DateTime.Now;
            Payload = JsonConvert.SerializeObject(data);
            Type = hookType.ToString();
        }
    }
}
