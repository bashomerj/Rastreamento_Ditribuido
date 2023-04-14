using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Validation
{
    public static class ValidationResultExtension
    {
        public static object Result { get; private set; } 

        public static void AtribuirResultado(this ValidationResult validationResult, object result)
        {
            Result = result;
        }

        public static object RetornarResultado(this ValidationResult validationResult)
        {
            return Result;
        }
    }
}
