using Microsoft.EntityFrameworkCore;
using Cliente.API.Data;
using Cliente.API.Models.Entities;
using Cliente.API.Models.Interfaces;
using Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cliente.API.Models.Repositories
{
    public class ClienteRepository : RepositoryCobrancaGeneric<Cliente.API.Models.Entities.Cliente> , IClienteRepository
    {
        private readonly CobrancaContext  _cobrancaContext;

        public ClienteRepository(CobrancaContext cobrancaContext) : base(cobrancaContext)
        {
            _cobrancaContext = cobrancaContext;
        }

        public IUnitOfWork UnitOfWork => _cobrancaContext;


        public void Dispose()
        {
            _cobrancaContext.Dispose();
        }
    }
}
