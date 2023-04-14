using SEG.Core.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalogo.API.Application.Events
{
    public class CriarSeguroAdicionadoEvent : Event
    {
        public int Contrato { get; set; }
        public int PropostaCorretor { get; set; }
        public DateTime InicioVigencia { get; set; }
        public DateTime VigenciaPlano { get; set; }
        public int CodigoDeEmissao { get; set; }
        public string NomeTitular { get; set; }
        public string EmailTitular { get; set; }
        public int Sucursal { get; set; }
        public short Produto { get; set; }
        public decimal CPF { get; set; }
        public short Plano { get; set; }
        public decimal PremioTotal { get; set; }

        public CriarSeguroAdicionadoEvent(int contrato, int propostaCorretor, DateTime inicioVigencia, DateTime vigenciaPlano, 
                                          int codigoDeEmissao, string nomeTitular, string emailTitular, int sucursal, short produto, 
                                          decimal cPF, short plano, decimal premioTotal
        )
        {
            Contrato = contrato;
            PropostaCorretor = propostaCorretor;
            InicioVigencia = inicioVigencia;
            VigenciaPlano = vigenciaPlano;
            CodigoDeEmissao = codigoDeEmissao;
            NomeTitular = nomeTitular;
            EmailTitular = emailTitular;
            Sucursal = sucursal;
            Produto = produto;
            CPF = cPF;
            Plano = plano;
            PremioTotal = premioTotal;
        }

    }
}
