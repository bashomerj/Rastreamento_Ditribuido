using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SEG.Core.Data;
using SEG.Webhook.API.Models.Entities;
using SEG.Webhook.API.Models.Interfaces;
using SEG.Webhook.API.Models.Repositories;

namespace SEG.Webhook.API.Models.Repositories
{
    public interface IWebHookSaidaRepository : IRepositoryGeneric<WebHookSaida>, IRepository<WebHookSaida>
    {
    }
}
