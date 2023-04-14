using FluentValidation.Results;
using SEG.Core.Messages.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


using Microsoft.AspNetCore.Mvc;
using SEG.Core.Mediator;
using SEG.WebAPI.Core.Controllers;
using SEG.WebAPI.Core.Usuario;
using SEG.Core.Enum;
using Cliente.API.Models.Repositories;
using Cliente.API.Services;

namespace Cliente.API.Services
{
    public class BaixaPagamentoService : IBaixaPagamentoService
    {

        public BaixaPagamentoService()
        {
            
        }

        public async Task<ValidationResult> TransferenciaPagamentoProposta(int empresa, int sucursal, string usuario, decimal nossoNumeroOrigem, decimal nossoNumeroDestino)
        {
            ValidationResult result = new ValidationResult();

            //var parcela_premio = await _parcelaPremioRepository.Obter(p => p.Cdconseg == contrato && p.Cdemi == emissao && p.Cdparpre == parcela+1);
            //if (parcela_premio != null)
            //{
            //    result.Errors.Add(new ValidationFailure("", "Já existe parcela gerada"));
            //    return result;
            //}



            try
            {
                //_parcelaPremioRepository.UnitOfWork.BeginTran();
                
                //var parcelaAnterior = parcela - 1; //tem que passar a parcela anterior para a proc de geração da parcela
                //var parcelaGerada = _parcelaPremioRepository.GerarParcelaCertificado(contrato, emissao, parcelaAnterior, data_vencimento, item);

                //_parcelaPremioRepository.UnitOfWork.CommitTran();

                return result;

            }
            catch (Exception)
            {
                throw;
            }

            
        }

       

    }





}
