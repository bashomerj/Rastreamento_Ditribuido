using SEG.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SEG.Bff.Web.DTO
{
    public class PlanoCoberturaDTO
    {
        public int Agrupamento { get; set; }
        public int Produto { get; set; }
        public int SequencialPlano { get; set; }
        public DateTime DataVigencia { get; set; }
        public Tipo_Segurado TipoSegurado { get; set; }
        public Sexo Sexo { get; set; }
        public int Idade { get; set; }
        public List<CoberturaDTO> Coberturas { get; set; }
        public Periodicidade PeriodicidadePagamento { get; set; }
        //public int Ramo { get; set; }
    }

    public class CoberturaDTO
    {
        public int CodigoCoberura { get; set; }
        public decimal IS { get; set; }
        public decimal Premio { get; set; }
    }





}
