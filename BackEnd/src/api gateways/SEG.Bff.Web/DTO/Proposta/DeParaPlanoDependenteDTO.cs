using System;
using System.ComponentModel.DataAnnotations;

namespace SEG.Bff.Web.DTO.Proposta
{
    public class DeParaPlanoDependenteDTO
    {
        [Required]
        public int produto { get; set; }
        [Required]
        public int plano { get; set; }
        [Required]
        public DateTime vigencia_Plano { get; set; }
        [Required]
        public string descricaoPlano { get; set; }

    }
}
