using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Results;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Bff.Web.Extensions
{
    public static class FluentvalidationExtension
    {
        public static AbstractValidator<T>  RetornaValor<T>(this AbstractValidator<T> item)
        {
            return item;
        }
    }
}

