using ProcessarProposta.Worker.Model.Entities;
using SEG.Cobranca.API.Data;
using Core.Data;

namespace SEG.Cobranca.API.Models.Repositories
{
    public class FluxoProcessamentoRepository : RepositoryGeneric<FluxoProcessamento>, IFluxoProcessamentoRepository
    {
        private readonly ParcelaContext _parcelaContext;


        public FluxoProcessamentoRepository(ParcelaContext parcelaContext) : base(parcelaContext)
        {
            _parcelaContext = parcelaContext;
        }

        public IUnitOfWork UnitOfWork => _parcelaContext;


        public void Dispose()
        {
            _parcelaContext.Dispose();
        }

        //private readonly SiesContext _siesContext;


        //public FluxoProcessamentoRepository(SiesContext siesContext) : base(siesContext)
        //{
        //    _siesContext = siesContext;
        //}

        //public IUnitOfWork UnitOfWork => _siesContext;


        //public void Dispose()
        //{
        //    _siesContext.Dispose();
        //}

    }
}