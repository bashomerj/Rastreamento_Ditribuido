using FluentValidation.Results;
using MediatR;
using SEG.Core.Messages;
using SEG.Seguro.API.Application.Events;
using SEG.Seguro.API.Models.Entities;
using SEG.Seguro.API.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SEG.Seguro.API.Application.Commands
{
    public class CriarSeguroCommandHandler : CommandHandler, IRequestHandler<CriarSeguroCommand, ValidationResult>
    {
        private readonly IEmissaoRepository _emissaoRepository;
        private readonly IControleDataSistemaRepository _controleDataSistemaRepository;
        private readonly IItemGenericoRepository _itemGenericoRepository;
        private readonly IAgrupamentoRepository _agrupamentoRepository;
        private readonly INumeradorDocRepository _numeradorDocRepository;
        private readonly ICertificadoRepository _certificadoRepository;

        public CriarSeguroCommandHandler(IEmissaoRepository emissaoRepository, IControleDataSistemaRepository controleDataSistemaRepository,
                                         IItemGenericoRepository itemGenericoRepository, IAgrupamentoRepository agrupamentoRepository,
                                         INumeradorDocRepository numeradorDocRepository, ICertificadoRepository certificadoRepository)
        {
            _emissaoRepository = emissaoRepository;
            _controleDataSistemaRepository = controleDataSistemaRepository;
            _itemGenericoRepository = itemGenericoRepository;
            _agrupamentoRepository = agrupamentoRepository;
            _numeradorDocRepository = numeradorDocRepository;
            _certificadoRepository = certificadoRepository;
        }

        public async Task<ValidationResult> Handle(CriarSeguroCommand seguroCommand, CancellationToken cancellationToken)
        {

            if (!seguroCommand.EhValido()) return seguroCommand.ValidationResult;


            // -------------------------------------------
            //  01. Emissão
            // -------------------------------------------

            var emissaoDoContrato = await _emissaoRepository.Obter(a => a.Cdconseg == seguroCommand.Contrato && a.Cdemi == 1);

            if (emissaoDoContrato == null)
            {
                AdicionarErro("Emissão não localizada para o contrato informado");
                return ValidationResult;
            }

            var dataDoSistema = _controleDataSistemaRepository.BuscarDataDoSistema();

            if (dataDoSistema == null)
            {
                AdicionarErro("A data do sistema não foi localizada");
                return ValidationResult;
            }

            var codigoDeEmissao = _emissaoRepository.GerarProximoNumeroDeEmissaoPorContrato(seguroCommand.Contrato);

            var emissaoDoCertificado = new Emissao(
                seguroCommand.Contrato,
                seguroCommand.Proposta,
                seguroCommand.Titular.InicioVigencia,
                dataDoSistema,
                codigoDeEmissao,
                emissaoDoContrato.Cdrmoseg,
                emissaoDoContrato.Cdsmo,
                emissaoDoContrato.Nrapo,
                emissaoDoContrato.Tpope,
                emissaoDoContrato.Tpeds,
                emissaoDoContrato.Dtemi,
                emissaoDoContrato.Pcnospte,
                emissaoDoContrato.Pcmdicom,
                emissaoDoContrato.Nratasor,
                emissaoDoContrato.Nrcmnsegvul,
                emissaoDoContrato.Nrordcosact,
                emissaoDoContrato.Tpnivcom,
                emissaoDoContrato.Tprcrrtt,
                emissaoDoContrato.Inrcliof,
                emissaoDoContrato.Incbajur,
                emissaoDoContrato.Incbacstapo,
                emissaoDoContrato.Txpdapre,
                emissaoDoContrato.Cdrefmonpre,
                emissaoDoContrato.Cdrefmonis,
                emissaoDoContrato.Vlistotinf,
                emissaoDoContrato.Dtatasor,
                emissaoDoContrato.Vlpretotemi,
                emissaoDoContrato.Tpprt,
                emissaoDoContrato.Stitf,
                emissaoDoContrato.Cdorgprtsuc,
                emissaoDoContrato.Tporgprtsuc,
                emissaoDoContrato.Cdorgprtemp,
                emissaoDoContrato.Tporgprtemp,
                emissaoDoContrato.Dtresseguro,
                emissaoDoContrato.Incancemi,
                emissaoDoContrato.Vllimind
                );

            _emissaoRepository.Adicionar(emissaoDoCertificado);



            // -------------------------------------------
            //  02. Produção Emissao
            // -------------------------------------------

            // Acertar com o Bruno
            var pessoaCorretor = seguroCommand.Colaborador;
            var cdPesins = 1; // pessoaInspetoria

            var producaoEmissao = new ProducaoEmissao(
                cdconseg: emissaoDoCertificado.Cdconseg,
                cdemi: codigoDeEmissao,
                cdPesins,
                cdorgprtsuc: seguroCommand.Sucursal,
                cdpescol: pessoaCorretor
                );

            _emissaoRepository.AdicionarProducaoEmissao(producaoEmissao);



            // -------------------------------------------
            //  03. Estado Emissao
            // -------------------------------------------

            var estadoEmissao = new EstadoEmissao(emissaoDoCertificado.Cdconseg, emissaoDoCertificado.Cdemi, seguroCommand.Usuario);

            _emissaoRepository.AdicionarEstadoEmissao(estadoEmissao);




            // -------------------------------------------
            //  04. Item Genérico
            // -------------------------------------------

            var codigoItemGenerico = _itemGenericoRepository
                                        .GerarProximoNumeroDeItemGenericoPorContrato(emissaoDoCertificado.Cdconseg);

            ItemGenerico itemGenerico = new ItemGenerico(
                cdconseg: emissaoDoCertificado.Cdconseg,
                cditeseg: codigoItemGenerico,
                cdagr: seguroCommand.Titular.Agrupamento,
                cdpes: 1,
                dsiteseg: seguroCommand.Titular.Pessoa.Nome,
                cdemi: codigoDeEmissao);


            _itemGenericoRepository.Adicionar(itemGenerico);




            // -------------------------------------------
            //  05. ItemOutRiscPess
            // -------------------------------------------

            var enquadramentoAutomatico = _agrupamentoRepository
                                            .BuscarEnquadramentoAutomatico(emissaoDoCertificado.Cdconseg,
                                                                           seguroCommand.Titular.Agrupamento);

            // ----------------------------------------------------------
            //      5.1. Cadastramento de um tipo de Segurado: Titular
            // ----------------------------------------------------------


            if (seguroCommand.Titular != null)
            {
                var itemOutRiscPessTitular = ItemOutRiscPess.Titular(
                    cdconseg: seguroCommand.Contrato,
                    cditeseg: codigoItemGenerico,
                    dtsitseg: dataDoSistema,
                    inenqaut: (char)enquadramentoAutomatico,
                    cdprodut: seguroCommand.Titular.Produto,
                    nrseqplnind: 1,
                    cdagr: seguroCommand.Titular.Agrupamento,
                    cdpes: 1,
                    dtinivig: seguroCommand.Titular.InicioVigencia,
                    nrcgccpf: seguroCommand.Titular.Pessoa.CPF
                );

                _itemGenericoRepository.AdicionandoItemOutRisPess(itemOutRiscPessTitular);
            }
            
            


            // -------------------------------------------
            //  06. Certificado
            // -------------------------------------------


            // Gerar o numero do certificado
            var numerador = await _numeradorDocRepository.Obter("CertificadoVida");
            numerador.AdicionarNumerador();

            // Atualizar o número do certificado +1
            _numeradorDocRepository.Atualizar(numerador);

            var codigoCertificado = (int)numerador.Nrultuti;

            var item = _itemGenericoRepository.GerarProximoNumeroDeItemGenericoPorContrato(emissaoDoCertificado.Cdconseg);

            // envolve o plano individual em grupo
            var quantidadeParcelaCarne = 1; // Select tpfreqpl From PlanoIndGrupo Where cdprodut == Produto And dtinivig == InicioVigencia And nrseqplnind...

            //verificar pelo tipo do segurado
            var conjuge = 'N';

            var vendaAdministrativa = seguroCommand.Titular.VendaAdministrativa != null ? 'S' : 'N';

            // O Bruno disse que isso vai morrer - 27/10/2021
            var migracaoAutomaticaDependente = 'N';


            //isnull(c.idmigdep,'N'),

            var certificado = new Certificado(
                emissaoDoCertificado.Cdconseg,
                codigoDeEmissao,
                codigoCertificado,
                item,
                (short)quantidadeParcelaCarne,
                seguroCommand.Titular.Periodicidade,
                seguroCommand.Titular.MesesParaRenda,
                vendaAdministrativa,
                migracaoAutomaticaDependente,
                conjuge
                );

            _certificadoRepository.Adicionar(certificado);



            // -------------------------------------------
            //  07. Certificado Digital
            // -------------------------------------------

            var isDigital = seguroCommand.Titular.Digital.ToUpper() == "SIM" ? 'S' : 'N';

            var certificadoDigital = new CertificadoDigital(
                emissaoDoCertificado.Cdconseg,
                codigoDeEmissao,
                codigoCertificado,
                isDigital
                );

            _certificadoRepository.AdicionarCertificadoDigital(certificadoDigital);



            // -------------------------------------------
            //  08. Venda Administrativa
            // -------------------------------------------

            if (vendaAdministrativa == 'S')
            {
                var cert = await _certificadoRepository
                                    .Obter(c => c.Cdconseg == seguroCommand.Titular.VendaAdministrativa.ContratoOriginal &&
                                        c.Nrcer == seguroCommand.Titular.VendaAdministrativa.CertificadoOriginal);

                var certificadoVD = new CertificadoVendaAdministrativa(
                    emissaoDoCertificado.Cdconseg,
                    codigoDeEmissao,
                    codigoCertificado,
                    seguroCommand.Titular.VendaAdministrativa.Motivo,
                    cert.Cdconseg,
                    cert.Cdemi,
                    cert.Nrcer
                    );

                _certificadoRepository.AdicionarCertificadoVendaAdministrativa(certificadoVD);

            }


            // -------------------------------------------
            //  09. Recompra
            // -------------------------------------------

            var cdpes_2 = 1; // Preciso buscar em pessoa Select cdpes From Pessoa Where nrcgccpf == seguroCommand.Titular.Pessoa.CPF

            var recompra = _certificadoRepository.IdentificarRecompra(cdpes_2);

            if (recompra)
            {
                var certificadoRecompra = new CertificadoRecompra(
                emissaoDoCertificado.Cdconseg,
                codigoDeEmissao,
                codigoCertificado
                );

                _certificadoRepository.AdicionarCertificadoRecompra(certificadoRecompra);
            }



            // --------------------------------------------------------
            //  10. Registrar Evento de SeguroCommand Alterado
            // --------------------------------------------------------

            emissaoDoCertificado.AdicionarEvento(new CriarSeguroAdicionadoEvent(
                emissaoDoCertificado.Cdconseg,
                seguroCommand.Proposta,
                seguroCommand.Titular.InicioVigencia,
                seguroCommand.Titular.VigenciaPlano,
                codigoDeEmissao,
                seguroCommand.Titular.Pessoa.Nome,
                seguroCommand.Titular.Pessoa.Email,
                seguroCommand.Sucursal,
                seguroCommand.Titular.Produto,
                seguroCommand.Titular.Pessoa.CPF,
                seguroCommand.Titular.Plano,
                seguroCommand.Titular.PremioTotal
            ));

            var resultEmissao = await PersistirDados(_emissaoRepository.UnitOfWork);

            return ValidationResult;

        }
    }
}
