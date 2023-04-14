using SEG.Core.Data;
using SEG.Webhook.API.Models.Entities;
using SEG.Webhook.API.Models.Repositories;


namespace SEG.Webhook.API.Data.Repositories
{
    public class AssinaturaWebhookRepository : RepositoryGeneric<AssinaturaWebhook>, IAssinaturaWebhookRepository
    {
        private readonly WebHookContext _webHookContext;
        public AssinaturaWebhookRepository(WebHookContext webHookContext) : base(webHookContext)
        {
            _webHookContext = webHookContext;
        }

        public IUnitOfWork UnitOfWork => _webHookContext;

        public void Dispose()
        {
            _webHookContext.Dispose();
        }
    }
}
