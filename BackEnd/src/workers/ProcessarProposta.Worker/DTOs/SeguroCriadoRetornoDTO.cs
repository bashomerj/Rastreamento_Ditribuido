using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcessarProposta.Worker.DTOs
{
    public class SeguroCriadoRetornoDTO
    {
        public int contrato { get; set; }
        public int proposta { get; set; }
        public int certificado { get; set; }
        public int emissao { get; set; }
        public int item { get; set; }
        public short situacao_proposta { get; set; }
        public List<int> certificado_agregados { get; set; }

    }


}
