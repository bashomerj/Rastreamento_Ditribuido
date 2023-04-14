using System.ComponentModel.DataAnnotations;

namespace Bff.Web.DTO.Contrato
{
   public class ContratoEmpresaDTO
    {
        [Required]
        public int contrato { get; set; }
        [Required]
        public int empresa { get; set; }
        [Required]
        public int sucursal { get; set; }
    }
}
