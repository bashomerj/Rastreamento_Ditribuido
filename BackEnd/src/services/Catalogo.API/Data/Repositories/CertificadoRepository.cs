using SEG.Core.Data;
using Catalogo.API.Data;
using Catalogo.API.Models.Entities;
using Catalogo.API.Models.Interfaces;
using Catalogo.API.Models.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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