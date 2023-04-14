using System;

namespace Bff.Web.DTO
{
    public class CertificadoPorPropostaDTO
    {
        public int contrato { get; set; }
        public int certificado { get; set; }
        public int emissao { get; set; }
        public short situacao { get; set; }
        public DateTime inicio_vigencia { get; set; }
        public DateTime fim_vigencia { get; set; }
        public int item { get; set; }
        public int? codigo_pessoa { get; set; }
        public string permite_redigitacao { get; set; }
    }
}
