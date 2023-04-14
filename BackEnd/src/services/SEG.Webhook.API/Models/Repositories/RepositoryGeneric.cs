using Microsoft.EntityFrameworkCore;
using SEG.Webhook.API.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SEG.Webhook.API.Models.Repositories
{
    public class RepositoryGeneric<T> : IRepositoryGeneric<T> where T : class
    {
        private readonly WebHookContext _webHookContext;

        public RepositoryGeneric(WebHookContext webHookContext)
        {
            _webHookContext = webHookContext;
        }

        public async Task<T> Obter(Expression<Func<T, bool>> predicated, params string[] includes)
        {
            var result = _webHookContext.Set<T>().Where(predicated).AsNoTracking();
            foreach (var item in includes)
            {
                result = result.Include(item);
            }
            return await result.FirstOrDefaultAsync();
        }

        public async Task Adicionar(T t)
        {
            await _webHookContext.AddAsync(t);
        }

        public void Atualizar(T t)
        {
            _webHookContext.Update(t);
        }

        public IEnumerable<T> ObterLista(Expression<Func<T, bool>> predicated, params string[] includes)
        {
            var result = _webHookContext.Set<T>().Where(predicated).AsNoTracking();
            foreach (var item in includes)
            {
                result = result.Include(item);
            }

            return result.AsEnumerable();
        }

        public void Remover(T obj)
        {
            _webHookContext.Set<T>().Remove(obj);
        }

        public void RemoverTodos(Expression<Func<T, bool>> predicated)
        {
            var result = _webHookContext.Set<T>().Where(predicated).AsNoTracking();
            _webHookContext.Set<T>().RemoveRange(result);
        }

        public IEnumerable<T> ExecuteStoredProcedureList(string commandText, params string[] parameters)
        {
            //   _context.Database.SetCommandTimeout(360);
            //     _context.Database.ExecuteSqlRaw(commandText, parameters);
            var result = _webHookContext.Set<T>()
                    .FromSqlRaw(commandText, parameters).AsNoTracking();
            return result;
        }

        public IEnumerable<T> ExecuteStoredProcedureList(string commandText)
        {
            var result = _webHookContext.Set<T>()
                      .FromSqlRaw(commandText).AsNoTracking();
            return result;
        }

        public void ExecuteStoredProcedure(string commandText, params string[] parameters)
        {
            _webHookContext.Database.ExecuteSqlRaw(commandText, parameters);
        }

       
    }
}
