using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SEG.Core.Enum;

namespace SEG.Bff.Web.DTO
{
    public class CadastrarPessoaSeguradoDTO
    {
        public Tipo_Segurado TipoSegurado { get; set; }
        public string Nome { get; set; }
        public Guid Id { get; set; }
        public int Cdpes { get; set; }
        public DateTime DataNascimento { get; set; }
        public Sexo Sexo { get; set; }
        public string CPF { get; set; }
        public Cpf_Proprio? CPFProprio { get; set; }
        public decimal? Renda { get; set; }
        public string Atividade { get; set; }
        public Estado_Civil? EstadoCivil { get; set; }
        public string Email { get; set; }
        public string RG { get; set; }
        public string OrgaoExpedidor { get; set; }
        public DateTime DataExpedicao { get; set; }
        public List<TelefoneDTO> Telefone { get; set; }
        public EnderecoDTO Endereco { get; set; }
        public string CodigoUsuario { get; set; }
        public int CodigoEmpresa { get; set; }
        public int CodigoSucursal { get; set; }
        public DateTime DataVigenciaSeguro { get; set; }

    }


}
