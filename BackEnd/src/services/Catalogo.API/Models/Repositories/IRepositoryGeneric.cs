using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Catalogo.API.Models.Repositories
{
    public interface IRepositoryGeneric<T> where T : class
    {
        Task<T> Obter(Expression<Func<T, bool>> predicated, params string[] includes);
        Task Adicionar(T t);
        void Atualizar(T t);

        IEnumerable<T> ObterLista(Expression<Func<T, bool>> predicated, params string[] includes);

        void Remover(T obj);

        void RemoverTodos(Expression<Func<T, bool>> predicated);

        IEnumerable<T> ExecuteStoredProcedureList(string commandText, params string[] parameters);
        IEnumerable<T> ExecuteStoredProcedureList(string commandText);
        void ExecuteStoredProcedure(string commandText, params string[] parameters);
        
    }
}
