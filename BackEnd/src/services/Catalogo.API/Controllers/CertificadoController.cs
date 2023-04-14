using EasyNetQ.LightInject;
using Microsoft.AspNetCore.Mvc;
using Core.Utils;
using Catalogo.API.DTO;
using Catalogo.API.Models.Repositories;
using WebAPI.Core.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalogo.API.Controllers
{
    [Route("api/seguro/certificado")]
    public class CertificadoController : MainController
    {
        private readonly ICertificadoRepository _certificadoRepository;


        public CertificadoController(ICertificadoRepository certificadoRepository)
        {
            _certificadoRepository = certificadoRepository;
        }

        [HttpPost("validar-venda-administrativa")]
        public async Task<IActionResult> ValidarVendaAdministrativa(ValidarVendaAdministrativaDTO vendaAdministrativa)
        {

            //var motivos = _dominioCamporepository.ObterLista(d => d.Nm_tab == "CertificadoVendaAdministrativa");
            //bool motivoExistente = false;
            //foreach (var item in motivos)
            //{
            //    if (StringUtils.RemoverAcentuacaoEspacoUpper(item.Ds_sgncam) == StringUtils.RemoverAcentuacaoEspacoUpper(vendaAdministrativa.Motivo.ToString()))
            //    {
            //        motivoExistente = true;
            //        break;
            //    }
            //}
            //if (!motivoExistente)
            //    AdicionarErroProcessamento("error_id: motivo_venda_administrativa_nao_encontrato - Motivo da venda administrativa não foi encontrado");
            

            //if (vendaAdministrativa.CertificadoOriginal == 0) AdicionarErroProcessamento("error_id: certificado_original_venda_administrativa_nao_informado - CertificadoOriginal da venda administrativa não foi informado");

            //if (vendaAdministrativa.ContratoOriginal == 0) AdicionarErroProcessamento("error_id: contrato_original_venda_administrativa_não_informado - ContratoOriginal da venda administrativa não foi informado");

            

            //if (!this.OperacaoValida())
            //    return CustomResponse();


            //var contratoSeguro = await _contratoSeguroRepository.Obter(c => c.Cdconseg == vendaAdministrativa.ContratoOriginal);
            //if (contratoSeguro == null) AdicionarErroProcessamento("error_id: contrato_original_venda_administrativa_não_encontrato - ContratoOriginal da venda administrativa não encontrado");
            //if (contratoSeguro.ValidarContratoVidaIndividual())
            //{
            //    var certificado = await _certificadoRepository.Obter(c => c.Cdconseg == vendaAdministrativa.ContratoOriginal && c.Nrcer == vendaAdministrativa.CertificadoOriginal, "Emissao", "ItemOutRiscPess");

            //    if (certificado == null)
            //    {
            //        this.AdicionarErroProcessamento($"error_id: certificado_original_venda_administrativa_nao_encontrato - Certificado orignal {vendaAdministrativa.ContratoOriginal}/{vendaAdministrativa.CertificadoOriginal} não encontrado");
            //        return CustomResponse();
            //    }

            //    if (!certificado.Emissao.VerificarCancelado()) this.AdicionarErroProcessamento($"error_id: certificado_original_venda_administrativa_nao_cancelado - O certificado original {certificado.Cdconseg}/{certificado.Nrcer} não está cancelado.");

            //    if (!certificado.ItemOutRiscPess.VerificarTitular()) this.AdicionarErroProcessamento($"error_id: certificado_original_venda_administrativa_nao_pertence_titular - O certificado original {certificado.Cdconseg}/{certificado.Nrcer} não pertence a um titular.");
            //}
            //else
            //{
            //    var certificado = await _certificadoRepository.Obter(c => c.Cdconseg == vendaAdministrativa.ContratoOriginal && c.Nrcer == vendaAdministrativa.CertificadoOriginal, "ItemOutRiscPess");

            //    if (certificado == null)
            //    {
            //        this.AdicionarErroProcessamento($"error_id: certificado_original_venda_administrativa_nao_encontrato - Certificado orignal {vendaAdministrativa.ContratoOriginal}/{vendaAdministrativa.CertificadoOriginal} não encontrado");
            //        return CustomResponse();
            //    }

            //    if (!certificado.ItemOutRiscPess.VerificarItemAtivo()) this.AdicionarErroProcessamento($"error_id: certificado_original_venda_administrativa_nao_cancelado - O certificado original {certificado.Cdconseg}/{certificado.Nrcer} não está cancelado.");

            //    if (!certificado.ItemOutRiscPess.VerificarTitular()) this.AdicionarErroProcessamento($"error_id: certificado_original_venda_administrativa_nao_pertence_titular - O certificado original {certificado.Cdconseg}/{certificado.Nrcer} não pertence a um titular.");
            //}

            return CustomResponse();

        }


        [HttpGet("por-proposta")]
        public async Task<IActionResult> RetornarPlano(
           [FromQuery] int empresa,
           [FromQuery] int sucursal,
           [FromQuery] int contrato,
           [FromQuery] int proposta)
        {

            //if (empresa == 0) AdicionarErroProcessamento("error_id: empresa_nao_informada - Empresa não informada");
            //if (sucursal == 0) AdicionarErroProcessamento("error_id: sucursal_nao_preenchida - Sucursal não informada");
            //if (contrato == 0) AdicionarErroProcessamento("error_id: contrato_nao_preenchido - Contrato não informado");
            //if (proposta == 0) AdicionarErroProcessamento("error_id: proposta_nao_preenchida - Proposta não informada");

            //if (!this.OperacaoValida())
            //    return CustomResponse();


            //var certificado = await _certificadoRepository.Obter(
            //    c => c.Emissao.Nrppscor == proposta &&
            //    c.Emissao.Cdconseg == contrato &&
            //    c.ItemOutRiscPess.Insegprin == "S" &&
            //    c.Emissao.ControleProposta.ControlePropostaOrgaoProdutor.Nr_ppt == proposta &&
            //    c.Emissao.ControleProposta.ControlePropostaOrgaoProdutor.Tp_orgpdr == 2 &&
            //    c.Emissao.ControleProposta.ControlePropostaOrgaoProdutor.Cd_orgpdr == sucursal,
            //    "Emissao", "ItemOutRiscPess", "Emissao.ControleProposta", "Emissao.ControleProposta.ControlePropostaOrgaoProdutor");

            //if (certificado == null)
            //    return NotFound(null);

            //return CustomResponse(new
            //{
            //    contrato = certificado.Cdconseg,
            //    certificado = certificado.Nrcer,
            //    emissao = certificado.Cdemi,
            //    situacao = certificado.Emissao.Tpestemi,
            //    inicio_vigencia = certificado.Emissao.Dtinivig,
            //    fim_vigencia = certificado.Emissao.Dtfimvig,
            //    item = certificado.Cditeseg,
            //    codigo_pessoa = certificado.ItemOutRiscPess.Cdpes,
            //    permite_redigitacao = certificado.Emissao.Idrecusared == null ? "N" : certificado.Emissao.Idrecusared.ToString()
            //});

            return CustomResponse();
        }
    }
}
