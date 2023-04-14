using System;
using System.Collections.Generic;
using SEG.Core.Enum;
using System.Linq;
using System.Threading.Tasks;

namespace SEG.Bff.Web.DTO
{
    public class CriarContratoDTO
    {

        public Tipo_Segurado TipoPessoa { get; set; } // Titular, Conjuge, Filho, Agregado

        public int Empresa { get; set; }
        public int Sucursal { get; set; }
        public int Contrato { get; set; }
        public int Proposta { get; set; }
        public short Colaborador { get; set; }
        public string Usuario { get; set; }
        
        public int Cdpes { get; set; }
        public string Nome { get; set; }
        public Sexo Sexo { get; set; }
        public string CPF { get; set; }
        public Cpf_Proprio CPFProprio { get; set; }
        public DateTime DataNascimento { get; set; }
        public Tipo_Segurado TipoAgregado { get; set; }
        public Grau_de_Parentesco GrauParentesco { get; set; }
        //public string Parentesco { get; set; }
        public decimal? Renda { get; set; }
        public string Email { get; set; }
        public Digital Digital { get; set; }

        public int? Emissao { get; set; }
        public DateTime InicioVigencia { get; set; }
        public int Agrupamento { get; set; }
        public short Produto { get; set; }
        public short Plano { get; set; }
        public DateTime VigenciaPlano { get; set; }
        public decimal PremioTotal { get; set; }
        public short MesesParaRenda { get; set; }
        public Periodicidade Periodicidade { get; set; }
        public VendaAdministrativaDTO VendaAdministrativa { get; set; }
        public decimal PercentualParticipacao { get; set; }
        public List<CoberturasDTO> Coberturas { get; set; }
        public List<DPSDTO> Dps { get; set; }

    }
}
