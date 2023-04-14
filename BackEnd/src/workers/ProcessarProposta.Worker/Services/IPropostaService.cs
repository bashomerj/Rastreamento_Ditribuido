using SEG.Cobranca.API.Models.Repositories;
using Core.Communication;
using Core.Messages.Integration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProcessarProposta.Worker.Services
{
    public interface IPropostaService
    {

        Task<ResponseResult> ProcessarProposta(PropostaValidadaIntegrationEvent message);
    }
}
