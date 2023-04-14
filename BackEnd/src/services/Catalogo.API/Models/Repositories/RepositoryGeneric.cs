using Microsoft.EntityFrameworkCore;
using Catalogo.API.Data;
using Catalogo.API.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Catalogo.API.Models.Repositories
{
    public class RepositoryGeneric<T> : IRepositoryGeneric<T> where T : class
    {
        private readonly SeguroContext _seguroContext;

        public RepositoryGeneric(SeguroContext seguroContext)
        {
            _seguroContext = seguroContext;
        }

        public async Task<T> Obter(Expression<Func<T, bool>> predicated, params string[] includes)
        {
            var result = _seguroContext.Set<T>().Where(predicated).AsNoTracking();
            foreach (var item in includes)
            {
                result = result.Include(item);
            }
            return await result.FirstOrDefaultAsync();
        }

        public async Task Adicionar(T t)
        {
            await _seguroContext.AddAsync(t);
        }

        public void Atualizar(T t)
        {
            _seguroContext.Update(t);
        }

        public IEnumerable<T> ObterLista(Expression<Func<T, bool>> predicated, params string[] includes)
        {
            var result = _seguroContext.Set<T>().Where(predicated).AsNoTracking();
            foreach (var item in includes)
            {
                result = result.Include(item);
            }

            return result.AsEnumerable();
        }

        public void Remover(T obj)
        {
            _seguroContext.Set<T>().Remove(obj);
        }

        public void RemoverTodos(Expression<Func<T, bool>> predicated)
        {
            var result = _seguroContext.Set<T>().Where(predicated).AsNoTracking();
            _seguroContext.Set<T>().RemoveRange(result);
        }

        public IEnumerable<T> ExecuteStoredProcedureList(string commandText, params string[] parameters)
        {
            //   _context.Database.SetCommandTimeout(360);
            //     _context.Database.ExecuteSqlRaw(commandText, parameters);
            var result = _seguroContext.Set<T>()
                    .FromSqlRaw(commandText, parameters).AsNoTracking();
            return result;
        }

        public IEnumerable<T> ExecuteStoredProcedureList(string commandText)
        {
            var result = _seguroContext.Set<T>()
                      .FromSqlRaw(commandText).AsNoTracking();
            return result;
        }

        public void ExecuteStoredProcedure(string commandText, params string[] parameters)
        {
            _seguroContext.Database.ExecuteSqlRaw(commandText, parameters);
        }

        //public void BeginTrans()
        //{
        //    _seguroContext.Database.BeginTransactionAsync();
        //}

        //public void CommitTrans()
        //{
        //    _seguroContext.SaveChangesAsync();
        //    _seguroContext.Database.CommitTransaction();
        //}

        //public void RollbackTrans()
        //{
        //    _seguroContext.Database.RollbackTransaction();
        //}

     
    }
}
