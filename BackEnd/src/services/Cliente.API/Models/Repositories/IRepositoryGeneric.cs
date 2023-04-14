using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Cliente.API.Models.Repositories
{
    public interface IRepositoryGeneric<T> where T : class
    {
        Task<T> Obter(Expression<Func<T, bool>> predicated, params string[] includes);
    }
}
