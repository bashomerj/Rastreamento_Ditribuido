using SEG.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalogo.API.DTO
{


    public class ValidarVendaAdministrativaDTO
    {
        public int ContratoOriginal { get; set; }
        public int CertificadoOriginal { get; set; }
        public Motivo_Venda_Administrativa Motivo { get; set; }

    }

 


   

}
