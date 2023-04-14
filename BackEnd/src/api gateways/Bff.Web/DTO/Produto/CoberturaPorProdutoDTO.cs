using System.ComponentModel.DataAnnotations;

namespace Bff.Web.DTO.Produto
{
    public class CoberturaPorProdutoDTO
    {
        [Required]
        public int cobertura { get; set; }
        [Required]
        public string nome { get; set; }
        [Required]
        public int classificacao { get; set; }
        [Required]
        public string descricaoClassificacao { get; set; }
        [Required]
        public string funeral { get; set; }

    }
}
