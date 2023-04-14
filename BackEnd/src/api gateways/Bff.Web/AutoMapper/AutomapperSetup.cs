using AutoMapper;
using Bff.Web.DTO;
using Core.Messages.Integration;

namespace Bff.Web.Automapper
{
    public class AutomapperSetup : Profile
    {
        public AutomapperSetup()
        {
            CreateMap<SeguradoDTO, CadastrarPessoaSeguradoDTO>()
                .ForMember(d => d.TipoSegurado, o => o.MapFrom(x => x.Tipo_Segurado))
                .ForMember(d => d.Id, o => o.MapFrom(x => x.Pessoa.Id))
                .ForMember(d => d.Nome, o => o.MapFrom(x => x.Pessoa.Nome))
                .ForMember(d => d.Cdpes, o => o.MapFrom(x => x.Pessoa.Cdpes))
                .ForMember(d => d.DataNascimento, o => o.MapFrom(x => x.Pessoa.Data_Nascimento))
                .ForMember(d => d.Sexo, o => o.MapFrom(x => x.Pessoa.Sexo))
                .ForMember(d => d.CPF, o => o.MapFrom(x => x.Pessoa.CPF))
                .ForMember(d => d.CPFProprio, o => o.MapFrom(x => x.Pessoa.CPF_Proprio))
                .ForMember(d => d.Renda, o => o.MapFrom(x => x.Pessoa.Renda))
                .ForMember(d => d.Atividade, o => o.MapFrom(x => x.Pessoa.Atividade))
                .ForMember(d => d.EstadoCivil, o => o.MapFrom(x => x.Pessoa.Estado_Civil))
                .ForMember(d => d.Email, o => o.MapFrom(x => x.Pessoa.Email))
                .ForMember(d => d.RG, o => o.MapFrom(x => x.Pessoa.RG))
                .ForMember(d => d.OrgaoExpedidor, o => o.MapFrom(x => x.Pessoa.Orgao_Expedidor))
                .ForMember(d => d.DataExpedicao, o => o.MapFrom(x => x.Pessoa.Data_Expedicao))
                .ForMember(d => d.Telefone, o => o.MapFrom(x => x.Pessoa.Telefones))
                .ForMember(d => d.Endereco, o => o.MapFrom(x => x.Pessoa.Endereco));

            CreateMap<SeguradoDTO, InfoSeguradoDTO>();
            CreateMap<PessoaDTO, InfoPessoaDTO>();
            CreateMap<SeguroDTO, InfoSeguroDTO>();
            CreateMap<NovaPropostaDTO, CriarSeguroDTO>();

            CreateMap<NovaPropostaDTO, PropostaValidadaIntegrationEvent>();
            CreateMap<SeguroDTO , SeguroValidado>();
            CreateMap<SeguradoDTO, SeguradoValidado>();
            CreateMap<PessoaDTO , PessoaValidada>();
            CreateMap<EnderecoDTO, EnderecoValidado>();
            CreateMap<CoberturasDTO , CoberturasValidadas>();
            CreateMap<VendaAdministrativaDTO, VendaAdministrativaValidada>();
            CreateMap<DPSDTO , DPSValidada>();
            CreateMap<TelefoneDTO, TelefoneValidado>();
            CreateMap<BeneficiarioDTO, BeneficiarioValidado>();
            CreateMap<TransferenciaPagamentoDTO, TransferenciaPagamentoValidada>().ReverseMap();
            CreateMap<MeioPagamentoDTO, MeioPagamentoValidada>().ReverseMap();
            CreateMap<DebitoAutomaticoDTO, DebitoAutomaticoValidada>().ReverseMap();


            CreateMap<PropostaValidadaIntegrationEvent, NovaPropostaDTO>();
            CreateMap<SeguroValidado, SeguroDTO>();
            CreateMap<SeguradoValidado, SeguradoDTO>();
            CreateMap<PessoaValidada, PessoaDTO>();
            CreateMap<EnderecoValidado, EnderecoDTO>();
            CreateMap<CoberturasValidadas, CoberturasDTO>();
            CreateMap<VendaAdministrativaValidada, VendaAdministrativaDTO>();
            CreateMap<DPSValidada, DPSDTO>();
            CreateMap<TelefoneValidado, TelefoneDTO>();
            CreateMap<BeneficiarioValidado, BeneficiarioDTO>();


        }  
    }
}

              
                