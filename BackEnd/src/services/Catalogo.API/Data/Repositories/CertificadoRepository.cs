using Catalogo.API.Models.Entities;
using Catalogo.API.Models.Repositories;
using Core.Data;

namespace Catalogo.API.Data.Repositories
{
    public class CertificadoRepository : RepositoryGeneric<Certificado>, ICertificadoRepository
    {
        private readonly SeguroContext _seguroContext;

        public CertificadoRepository(SeguroContext seguroContext) : base(seguroContext)
        {
            _seguroContext = seguroContext;
        }



        public IUnitOfWork UnitOfWork => _seguroContext;


        public void Dispose()
        {
            _seguroContext.Dispose();
        }

    }
}