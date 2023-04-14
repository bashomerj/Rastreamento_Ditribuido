using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SEG.Bff.Web.DTO;
using SEG.Bff.Web.DTO.Contrato;
using SEG.Bff.Web.DTO.Proposta;
using SEG.Bff.Web.Extensions;
using SEG.Core.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SEG.Bff.Web.Services
{
    public interface ISeguroService
    {
        Task<ResponseResult> ValidarVidaIndividual(Object seguroDto);
        Task<ResponseResult> ValidarEnderecoHabilitadoVenda(ValidarEnderecoHabilitadoVendaDTO endereco);
        Task<ResponseResult> ValidarVendaAdministrativa(ValidarVendaAdministrativaDTO vendaAdministrativa);
        Task<ResponseResult> VerificarAgrupamentoAtivo(VerificarAgrupamentoAtivoDTO agrupamentoAtivo);
        Task<ResponseResult> CriarCertificado(CriarSeguroDTO listaContrato);
        Task<ResponseResult> ValidarPropostaExisteEmpresaSucursalPendenteOuLiberada(Object propostaDTO);
        Task<ResponseResult> ValidarPlanoSegurado(PlanoCoberturaDTO plano);
        Task<ResponseResult> ValidarDPS(validarDpsDTO dps);
        Task<ResponseResult> ValidarMeioPagamento(validarMeioPagamentoDTO meioPagamento);
        Task<ResponseResult> AlterarProposta(PropostaDTO proposta);
        Task<ResponseResult> CertificadoPorProposta(int empresa, int sucursal, int contrato, int proposta);
        Task<ResponseResult> TraduzirPlano(int produto,
            DateTime inicio_vigencia,
            string tipo_segurado,
            string sexo,
            int idade,
            List<int> lista_coberturas);

        Task<ResponseResult> ObterContratos(int produto);
        Task<ResponseResult> DeParaPlanoDenpendentes(int produto,
            int plano_titular,
            DateTime vigencia_plano_titular,
            int tipo_dependente,
            bool cobertura_renda_mensal,
            bool cobertura_assistencia_emergencial);

        Task<ResponseResult> ValidarTransferenciaPagamento(TransferenciaPagamentoPropostaValidacaoDTO transferenciaPagamento);

        Task<ResponseResult> ValidarDebitoAutomatico(DebitoAutomaticoDTO debitoAutomatico);

        Task<ResponseResult> ObterControleProposta(int empresa, int sucursal, int proposta);
    }

    public class SeguroService : Service, ISeguroService
    {
        private readonly HttpClient _httpClient;

        public SeguroService(HttpClient httpClient, IOptions<AppServicesSettings> settings)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(settings.Value.SeguroUrl);
        }

        public async Task<ResponseResult> ValidarVidaIndividual(Object contrato)
        {

            var contratoContent = ObterConteudo(contrato);

            var response = await _httpClient.PostAsync("/api/seguro/contrato/validar/vidaindividual", contratoContent);

            if (!TratarErrosResponse(response)) return await DeserializarObjetoResponse<ResponseResult>(response);

            return RetornoOk();
        }

        public async Task<ResponseResult> ValidarEnderecoHabilitadoVenda(ValidarEnderecoHabilitadoVendaDTO endereco)
        {
            var enderecoContent = ObterConteudo(endereco);

            var response = await _httpClient.PostAsync("api/seguro/contrato/verificar/endereco/habilitado/para/venda", enderecoContent);

            if (!TratarErrosResponse(response)) return await DeserializarObjetoResponse<ResponseResult>(response);

            return RetornoOk();
        }

        public async Task<ResponseResult> ValidarVendaAdministrativa(ValidarVendaAdministrativaDTO vendaAdministrativa)
        {
            var response = await _httpClient.PostAsync("api/seguro/certificado/validar-venda-administrativa", ObterConteudo(vendaAdministrativa));

            if (!TratarErrosResponse(response)) return await DeserializarObjetoResponse<ResponseResult>(response);

            return RetornoOk();
        }
        

        public async Task<ResponseResult> VerificarAgrupamentoAtivo(VerificarAgrupamentoAtivoDTO agrupamento)
        {
            var agrupamentoContent = ObterConteudo(agrupamento);

            var response = await _httpClient.PostAsync("/api/seguro/agrupamento/validar/entradaProposta", agrupamentoContent);

            if (!TratarErrosResponse(response)) return await DeserializarObjetoResponse<ResponseResult>(response);

            return RetornoOk();
        }


        //public async Task<ResponseResult> CriarCertificado(List<CriarContratoDTO> listaContrato)
        //{
        //    var ListaCriarSeguroDTOContent = ObterConteudo(listaContrato);

        //    var response = await _httpClient.PostAsync("/api/seguro/criar/certificado", ListaCriarSeguroDTOContent);

        //    if (!TratarErrosResponse(response)) return await DeserializarObjetoResponse<ResponseResult>(response);

        //    return RetornoOk();

        //}

        public async Task<ResponseResult> CriarCertificado(CriarSeguroDTO seguro)
        {
            ResponseResult retorno = new ResponseResult();
            var seguroContent = ObterConteudo(seguro);

            var response = await _httpClient.PostAsync("/api/seguro/criar/certificado", seguroContent);

            if (response.IsSuccessStatusCode)
            {
                var objetoSucesso = await DeserializarObjetoResponse<SeguroRetornoCriacaoDTO>(response);
                retorno.AtribuirResponseObject(objetoSucesso);
            }
            else
                retorno = await DeserializarObjetoResponse<ResponseResult>(response);

            return retorno;
        }

        public async Task<ResponseResult> ValidarPropostaExisteEmpresaSucursalPendenteOuLiberada(Object propostaDTO)
        {
            var propostaDTOContent = ObterConteudo(propostaDTO);

            var response = await _httpClient.PostAsync("api/seguro/proposta/validar/existe/empresa/sucursal/liberada", propostaDTOContent);

            if (!TratarErrosResponse(response)) return await DeserializarObjetoResponse<ResponseResult>(response);

            return RetornoOk();
        }


        public async Task<ResponseResult> ValidarPlanoSegurado(PlanoCoberturaDTO planoCobertura)
        {
            var _planoCobertura = ObterConteudo(planoCobertura);

            var response = await _httpClient.PostAsync("api/plano/validar/plano", _planoCobertura);

            if (!TratarErrosResponse(response)) return await DeserializarObjetoResponse<ResponseResult>(response);

            return RetornoOk();
        }

        public async Task<ResponseResult> ValidarDPS(validarDpsDTO dps)
        {

            var dpsContent = ObterConteudo(dps);    

            var response = await _httpClient.PostAsync("api/seguro/certificado/validar-dps", dpsContent);

            if (!TratarErrosResponse(response)) return await DeserializarObjetoResponse<ResponseResult>(response);

            return RetornoOk();
        }

        public async Task<ResponseResult> ValidarMeioPagamento(validarMeioPagamentoDTO meioPagamento)
        {

            var meioPagamentoContent = ObterConteudo(meioPagamento);

            var response = await _httpClient.PostAsync("api/seguro/proposta/validar-meio-pagamento", meioPagamentoContent);

            if (!TratarErrosResponse(response)) return await DeserializarObjetoResponse<ResponseResult>(response);

            return RetornoOk();
        }


        public async Task<ResponseResult> AlterarProposta(PropostaDTO proposta)
        {
            //var propostaContent = ObterConteudo(proposta);

            var response = await _httpClient.PutAsync("api/seguro/proposta/alterar", ObterConteudo(proposta));

            if (!TratarErrosResponse(response)) return await DeserializarObjetoResponse<ResponseResult>(response);

            return RetornoOk();
        }

        
        public async Task<ResponseResult> CertificadoPorProposta(int empresa, int sucursal, int contrato, int proposta)
        {

            ResponseResult responseResult = new ResponseResult();
            //var numCrachaContent = ObterConteudo(numCracha);

            var response = await _httpClient.GetAsync($"api/seguro/certificado/por-proposta?empresa={empresa}&sucursal={sucursal}&contrato={contrato}&proposta={proposta}");
                

            if (response.IsSuccessStatusCode)
            {
                responseResult.Status = (int)response.StatusCode;
                var objetoSucesso = await DeserializarObjetoResponse<CertificadoPorPropostaDTO>(response);
                responseResult.AtribuirResponseObject(objetoSucesso);
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                responseResult.Status = (int)response.StatusCode;
                return responseResult;
            }
            else
                responseResult = await DeserializarObjetoResponse<ResponseResult>(response);

            return responseResult;
            
            
        }

        public async Task<ResponseResult> TraduzirPlano(int produto,
            DateTime inicio_vigencia,
            string tipo_segurado,
            string sexo,
            int idade,
            List<int> lista_coberturas)
        {
            ResponseResult retorno = new ResponseResult();


            string listaCobertura = "";
            lista_coberturas.ForEach(delegate (int item)
            {
                listaCobertura += ($"lista_coberturas={item}&");
            });

            var response = await _httpClient.GetAsync($"api/plano/traduzir?produto={produto}&inicio_vigencia={inicio_vigencia.ToString("yyyy-MM-dd")}&tipo_segurado={tipo_segurado}&sexo={sexo}&idade={idade}&{listaCobertura}");
            if (!TratarErrosResponse(response)) return await DeserializarObjetoResponse<ResponseResult>(response);

            retorno.Status = (int)response.StatusCode;
            if (response.IsSuccessStatusCode)
            {
                var objetoSucesso = await DeserializarObjetoResponse<TraducaoPlanoDTO>(response);
                retorno.AtribuirResponseObject<TraducaoPlanoDTO>(objetoSucesso);
            }
            //else if (!(response.StatusCode == HttpStatusCode.NotFound))
            //    retorno = await DeserializarObjetoResponse<ResponseResult>(response);


            return retorno;
        }


        public async Task<ResponseResult> ObterContratos(int produto)
        {
            ResponseResult retorno = new ResponseResult();

            var response = await _httpClient.GetAsync($"api/seguro/contrato/por-produto?produto={produto}");
            if (!TratarErrosResponse(response)) return await DeserializarObjetoResponse<ResponseResult>(response);

            retorno.Status = (int)response.StatusCode;
            if (response.IsSuccessStatusCode)
            {
                var objetoSucesso = await DeserializarObjetoResponse<List<ContratoEmpresaDTO>>(response);
                retorno.AtribuirResponseObject<List<ContratoEmpresaDTO>>(objetoSucesso);
            }
            //else if (!(response.StatusCode == HttpStatusCode.NotFound))
            //    retorno = await DeserializarObjetoResponse<ResponseResult>(response);


            return retorno;
        }


        public async Task<ResponseResult> DeParaPlanoDenpendentes(int produto,
            int plano_titular,
            DateTime vigencia_plano_titular,
            int tipo_dependente,
            bool cobertura_renda_mensal,
            bool cobertura_assistencia_emergencial)
        {
            ResponseResult retorno = new ResponseResult();

            var endpoint = $"api/plano/de-para-plano-dependente?produto={produto}&plano_titular={plano_titular}&vigencia_plano_titular={vigencia_plano_titular.ToString("yyyy-MM-dd")}&tipo_dependente={tipo_dependente}&cobertura_renda_mensal={cobertura_renda_mensal}&cobertura_assistencia_emergencial={cobertura_assistencia_emergencial}";
            var response = await _httpClient.GetAsync(endpoint);
            if (!TratarErrosResponse(response)) return await DeserializarObjetoResponse<ResponseResult>(response);

            retorno.Status = (int)response.StatusCode;
            if (response.IsSuccessStatusCode)
            {
                var objetoSucesso = await DeserializarObjetoResponse<DeParaPlanoDependenteDTO>(response);
                retorno.AtribuirResponseObject<DeParaPlanoDependenteDTO>(objetoSucesso);
            }

            return retorno;
        }

        public async Task<ResponseResult> ValidarTransferenciaPagamento(TransferenciaPagamentoPropostaValidacaoDTO transferenciaPagamento)
        {
            ResponseResult retorno = new ResponseResult();

            var response = await _httpClient.GetAsync($"api/seguro/proposta/transferencia-pagamento/validar?" +
                $"proposta_origem={transferenciaPagamento.proposta_origem}" +
                $"&proposta_destino={transferenciaPagamento.proposta_destino}" +
                $"&valor_premio_origem={transferenciaPagamento.valor_premio_origem}" +
                $"&valor_premio_destino={transferenciaPagamento.valor_premio_destino}");

            if (!TratarErrosResponse(response)) return await DeserializarObjetoResponse<ResponseResult>(response);

            retorno.Status = (int)response.StatusCode;
            if (response.IsSuccessStatusCode)
            {
                var objetoSucesso = await DeserializarObjetoResponse<List<ContratoEmpresaDTO>>(response);
                retorno.AtribuirResponseObject<List<ContratoEmpresaDTO>>(objetoSucesso);
            }

            return retorno;
        }

        public async Task<ResponseResult> ValidarDebitoAutomatico(DebitoAutomaticoDTO debitoAutomatico)
        {
            ResponseResult retorno = new ResponseResult();

            var response = await _httpClient.GetAsync($"api/seguro/proposta/debito-automatico/validar?" +
                $"agencia={debitoAutomatico.agencia}" +
                $"&digito_agencia={debitoAutomatico.digito_agencia}" +
                $"&conta={debitoAutomatico.conta}" +
                $"&digito_conta={debitoAutomatico.digito_conta}" +
                $"&tipo={debitoAutomatico.tipo}" +
                $"&categoria={debitoAutomatico.categoria}" +
                $"&titular={debitoAutomatico.titular}" +
                $"&cpf_titular={debitoAutomatico.cpf_titular}");

            if (!TratarErrosResponse(response)) return await DeserializarObjetoResponse<ResponseResult>(response);

            retorno.Status = (int)response.StatusCode;

            return retorno;
        }


        
        public async Task<ResponseResult> ObterControleProposta(int empresa, int sucursal, int proposta)
        {
            ResponseResult retorno = new ResponseResult();

            var response = await _httpClient.GetAsync($"api/seguro/proposta?empresa={empresa}&sucursal={sucursal}&numero_proposta={proposta}");
            if (!TratarErrosResponse(response)) return await DeserializarObjetoResponse<ResponseResult>(response);

            retorno.Status = (int)response.StatusCode;
            if (response.IsSuccessStatusCode)
            {
                var objetoSucesso = await DeserializarObjetoResponse<ControlePropostaDTO>(response);
                retorno.AtribuirResponseObject<ControlePropostaDTO>(objetoSucesso);
            }
            return retorno;
        }
    }
   
}



