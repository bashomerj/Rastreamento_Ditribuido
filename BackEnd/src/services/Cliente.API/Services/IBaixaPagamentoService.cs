using FluentValidation.Results;
using Core.Messages.Integration;
using System;
using System.Threading.Tasks;

namespace Cliente.API.Services
{
    public interface IBaixaPagamentoService
    {
        Task<ValidationResult> TransferenciaPagamentoProposta(int empresa, int sucursal, string usuario, decimal nossoNumeroOrigem, decimal nossoNumeroDestino);
    }
}
