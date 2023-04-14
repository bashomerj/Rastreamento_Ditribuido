﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.DomainObjects;

namespace Core.Data
{
    public interface IRepository<T> : IDisposable where T : IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }
    }
}