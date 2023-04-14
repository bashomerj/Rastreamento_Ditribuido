using SEG.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cliente.API.DTO
{
    public class TransferenciaPagamentoDTO
    {
        public int empresa { get; set; }
        public int sucursal { get; set; }
        public string usuario { get; set; }
        public decimal nossoNumeroOrigem { get; set; }
        public decimal nossoNumeroDestino { get; set; }
        

    }

    
}
