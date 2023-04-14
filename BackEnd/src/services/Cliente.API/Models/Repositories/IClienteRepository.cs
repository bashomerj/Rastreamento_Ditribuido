using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SEG.Core.Data;
using Cliente.API.Models.Entities;
using Cliente.API.Models.Interfaces;

namespace Cliente.API.Models.Repositories
{
    public interface IClienteRepository : IRepositoryGeneric<Cliente.API.Models.Entities.Cliente>, IRepository<Cliente.API.Models.Entities.Cliente>
    {
       

    }

}
