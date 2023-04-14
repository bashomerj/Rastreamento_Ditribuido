using Core.Enum;

namespace Catalogo.API.DTO
{


    public class ValidarVendaAdministrativaDTO
    {
        public int ContratoOriginal { get; set; }
        public int CertificadoOriginal { get; set; }
        public Motivo_Venda_Administrativa Motivo { get; set; }

    }

 


   

}
