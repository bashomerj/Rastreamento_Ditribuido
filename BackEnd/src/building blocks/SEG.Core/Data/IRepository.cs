using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SEG.Core.DomainObjects;

namespace SEG.Core.Data
{
    public interface IRepository<T> : IDisposable where T : IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }
    }
}