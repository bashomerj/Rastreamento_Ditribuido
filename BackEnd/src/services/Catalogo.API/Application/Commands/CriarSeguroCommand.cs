using FluentValidation;
using Core.Enum;
using Core.Messages;
using Catalogo.API.DTO;
using Catalogo.API.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalogo.API.Application.Commands
{
    public class CriarSeguroCommand : Command
    {
        public int Empresa { get; set; }


        public override bool EhValido()
        {
            ValidationResult = new CriarSeguroValidation().Validate(this);
            return ValidationResult.IsValid;
        }

        public class CriarSeguroValidation : AbstractValidator<CriarSeguroCommand>
        {
            public CriarSeguroValidation()
            {
                RuleFor(s => s.Empresa).NotEmpty().WithMessage("A empresa não foi informada");
            }
        }
    }



    

}
