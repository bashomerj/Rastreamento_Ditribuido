using SEG.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcessarProposta.Worker.DTOs
{
    public class GerarParcelaDTO
    {
        public int contrato { get; set; }
        public int emissao { get; set; }
        public int certificado { get; set; }
        public int item { get; set; }
        public short parcela { get; set; }
        public DateTime data_vencimento { get; set; }

    }

    public class GerarListaParcelaDTO
    {
        public int contrato { get; set; }
        public int emissao { get; set; }
        public short parcela { get; set; }
        public DateTime data_vencimento { get; set; }
        public int item { get; set; }
        public string gerar_nosso_numero { get; set; }
        public Periodicidade periodicidade { get; set; }
        public int quantidade_parcela { get; set; }

    }
}
