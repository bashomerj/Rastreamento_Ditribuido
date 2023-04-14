using Microsoft.EntityFrameworkCore;
using Cliente.API.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Cliente.API.Models.Repositories
{
    public class RepositoryCobrancaGeneric<T> : IRepositoryGeneric<T> where T : class
    {
        private readonly CobrancaContext _cobrancaContext;

        public RepositoryCobrancaGeneric(CobrancaContext cobrancaContext)
        {
            _cobrancaContext = cobrancaContext;
        }

        public async Task<T> Obter(Expression<Func<T, bool>> predicated, params string[] includes)
        {
            var result = _cobrancaContext.Set<T>().Where(predicated).AsNoTracking();
            foreach (var item in includes)
            {
                result = result.Include(item);
            }
            return await result.FirstOrDefaultAsync();
        }
    }
}
