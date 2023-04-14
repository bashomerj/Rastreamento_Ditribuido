using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SEG.Bff.Web.DTO;
using SEG.Bff.Web.Extensions;
using SEG.Core.Communication;
using SEG.MessageBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SEG.Bff.Web.Services
{
    public interface IInternalService
    {
        Task<ResponseResult> ProcessarProposta(NovaPropostaDTO novaPropostaDTO);

    }

    public class InternalService : Service, IInternalService
    {

        private readonly ISeguroService _seguroService;
        private readonly IPessoaService _pessoaService;
        private readonly ICobrancaService _cobrancaService;
        private readonly IMapper _mapper;
        private readonly IMessageBus _bus;

        public InternalService(ISeguroService seguroService, IPessoaService pessoaService, ICobrancaService cobrancaService, IMapper mapper, IMessageBus bus)
        {
            _seguroService = seguroService;
            _pessoaService = pessoaService;
            _cobrancaService = cobrancaService;
            _mapper = mapper;
            _bus = bus;
        }

        public async Task<ResponseResult> ProcessarProposta(NovaPropostaDTO novaPropostaDTO)
        {
            // ----------------------------------------------------------------------------------------------------------
            //  Passando por todas as críticas, vamos criar um mensagem na fila para processamento do seguro (RabbitMQ)
            //  ProcessarFila();
            // ----------------------------------------------------------------------------------------------------------
            ResponseResult retorno = new ResponseResult();

            // Criando os clientes como segurados
            List<CadastrarPessoaSeguradoDTO> listaSegurados = criarListaSegurados(novaPropostaDTO);
            var resultado = await _pessoaService.CadastrarListaPessoaSegurado(listaSegurados);
            if (resultado.Errors.Mensagens.Any())
                return resultado;
            var cadastrarPessoa = resultado.ObterResponseObject<List<CadastrarPessoaSeguradoDTO>>();

            if (cadastrarPessoa == null || cadastrarPessoa?.Count() <= 0) throw new Exception("Ocorreu um problema no processamento do cadastro de clientes.");


            // Atribuit o CDPES de cada pessoa criada para a chamada de criação dos certificados
            novaPropostaDTO.Titular.Pessoa.Cdpes = cadastrarPessoa.Where(t => t.Id == novaPropostaDTO.Titular.Pessoa.Id).FirstOrDefault().Cdpes;
            novaPropostaDTO.Agregados?.ForEach(t => t.Pessoa.Cdpes = cadastrarPessoa.Where(p => p.Id == t.Pessoa.Id).FirstOrDefault().Cdpes);



            // Chamando a API para Criar o Seguro
            CriarSeguroDTO seguro = _mapper.Map<CriarSeguroDTO>(novaPropostaDTO);
            resultado = await _seguroService.CriarCertificado(seguro);
            if (resultado.Errors.Mensagens.Any())
                return resultado;
            var certificado = resultado.ObterResponseObject<SeguroRetornoCriacaoDTO>();
            if (certificado == null) throw new Exception("Ocorreu um problema no processamento da criação dos certificados.");


            
            //Chamando a API para Criar a primeira parcela
            GerarParcelaDTO parcela = new GerarParcelaDTO() 
            { 
                contrato = certificado.Titular.cdconseg, 
                emissao = certificado.Titular.cdemi, 
                certificado = certificado.Titular.nrcer, 
                item = certificado.Titular.cditeseg, 
                parcela = 0, 
                data_vencimento = novaPropostaDTO.Seguro.inicio_vigencia 
            };

            resultado = await _cobrancaService.GerarParcela(parcela);
            if (resultado.Errors.Mensagens.Any())
                return resultado;


            resultado.AtribuirResponseObject<SeguroRetornoCriacaoDTO>(certificado);

            return resultado;
        }

        private List<CadastrarPessoaSeguradoDTO> criarListaSegurados(NovaPropostaDTO novaPropostaDTO)
        {
            List<CadastrarPessoaSeguradoDTO> listaSegurados = new List<CadastrarPessoaSeguradoDTO>();

            var titular = _mapper.Map<CadastrarPessoaSeguradoDTO>(novaPropostaDTO.Titular);
            titular.CodigoUsuario = novaPropostaDTO.Usuario;
            titular.CodigoEmpresa = novaPropostaDTO.Empresa;
            titular.CodigoSucursal = novaPropostaDTO.Sucursal;
            titular.DataVigenciaSeguro = novaPropostaDTO.Seguro.inicio_vigencia;

            listaSegurados.Add(titular);

            if (novaPropostaDTO.Agregados != null)
            {
                foreach (var item in novaPropostaDTO.Agregados)
                {
                    var agregado = _mapper.Map<CadastrarPessoaSeguradoDTO>(item);
                    agregado.CodigoUsuario = novaPropostaDTO.Usuario;
                    agregado.CodigoEmpresa = novaPropostaDTO.Empresa;
                    agregado.CodigoSucursal = novaPropostaDTO.Sucursal;
                    agregado.DataVigenciaSeguro = novaPropostaDTO.Seguro.inicio_vigencia;

                    listaSegurados.Add(agregado);
                }
            }

            return listaSegurados;

        }

    }

}



