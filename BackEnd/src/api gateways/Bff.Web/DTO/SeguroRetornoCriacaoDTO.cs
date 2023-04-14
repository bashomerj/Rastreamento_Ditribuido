using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bff.Web.DTO
{
    public class SeguroRetornoCriacaoDTO
    {
        public CertificadoRetornoCriacaoDTO Titular { get; set; }
        public List<CertificadoRetornoCriacaoDTO> Agregados { get; set; }

        public SeguroRetornoCriacaoDTO()
        {
            Agregados = new List<CertificadoRetornoCriacaoDTO>();
        }

    }
    public class CertificadoRetornoCriacaoDTO
    {
        public int cdconseg { get; set; }
        public int cdemi { get; set; }
        public int cditeseg { get; set; }
        public int nrcer { get; set; }
        public string nome { get; set; }
        public DateTime dataNascimento { get; set; }
        public decimal? CPF { get; set; }
        public List<CoberturaRetornoCriacaoDTO> Coberturas { get; set; }

        public CertificadoRetornoCriacaoDTO()
        {
            Coberturas = new List<CoberturaRetornoCriacaoDTO>();
        }
    }

    public class CoberturaRetornoCriacaoDTO
    {
        public int Coberura { get; set; }
        public decimal IS { get; set; }
    }

}
