using System;

namespace Bff.Web.DTO.Proposta
{
    public class ControlePropostaDTO
    {
        public int Nrppscor { get; set; }

        public int? Cdconseg { get; set; }
        public DateTime? Dtprop { get; set; }
        public string Nmescritorio { get; set; }
        public string Nmusuari { get; set; } 
        public decimal Nrnossonumero { get;  set; }
        public int? Stprop { get;  set; }
        public DateTime? Dtvenda { get;  set; }
        public string Stmotivo { get;  set; }
        public DateTime? Dtstatus { get;  set; }
        public decimal? Vlpretot { get;  set; }
        public DateTime? Dtven { get;  set; }

        public int? Cdpescol { get;  set; }

        public short? Incopinter { get;  set; }
        public DateTime? Dtcopinter { get;  set; }
        public short? Incopdig { get;  set; }
        public DateTime? Dtcopdig { get; set; }
    }
}
