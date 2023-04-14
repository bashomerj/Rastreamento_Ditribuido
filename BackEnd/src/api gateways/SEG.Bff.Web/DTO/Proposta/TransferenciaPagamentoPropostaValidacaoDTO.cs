using SEG.Core.Enum;

namespace SEG.Bff.Web.DTO.Proposta
{
    public class TransferenciaPagamentoPropostaValidacaoDTO
    {
        public int proposta_origem { get; set; }
        public int proposta_destino { get; set; }
        public decimal valor_premio_origem { get; set; }
        public decimal valor_premio_destino { get; set; }
        public Meio_Pagamento meio_pagamento_proposta_destino { get; set; }
        

    }
}
