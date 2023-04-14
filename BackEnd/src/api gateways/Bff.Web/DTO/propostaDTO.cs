using Bff.Web.Attributes;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Bff.Web.DTO
{
    public class PropostaDTO
    {
        public int Empresa { get; set; }
        public int Sucursal { get; set; }
        public int proposta { get; set; }
        public int? contrato { get; set; }
        public string unidade { get; set; }
        public int situacao { get; set; }
        public DateTime dataVenda { get; set; }
        public string motivo { get; set; }
        public decimal valorPremio { get; set; }
        public DateTime dataVencimento { get; set; }
        public int? colaborador { get; set; }
        public string tipoCobranca { get; set; }
        //public DebitoAutomaticoDTO debitoAutomatico { get; set; }

        public readonly List<string> listaUnidades = new List<string> { "rio_branco", "baixada", "zona_norte", "zona_oeste", "grande_niteroi", "presidente_vargas", "escola_de_vendas", "televendas" };
    }

    

    public class PropostaInutilizadaDTO
    {
        [Required]
        [SwaggerSchema(description: "código da empresa")]
        [SwaggerSchemaExample("1000")]
        public int Empresa { get; set; }

        [Required]
        [SwaggerSchema(description: "código da sucursal")]
        [SwaggerSchemaExample("2000")]
        public int Sucursal { get; set; }

        [Required]
        [SwaggerSchema(description: "número da proposta que foi realizada a venda")]
        [SwaggerSchemaExample("111111")]
        public int proposta { get; set; }

        [Required]
        [SwaggerSchema(description: "unidade que realizou a venda")]
        [SwaggerSchemaExample("rio_branco, baixada, zona_norte, zona_oeste, grande_niterói, presidente_vargas, escola_de_vendas, televendas")]
        public string unidade { get; set; }

        [Required]
        [SwaggerSchema(description: "Data que a venda foi realizada")]
        [SwaggerSchemaExample("2023-01-01")]
        public DateTime dataVenda { get; set; }

        [Required]
        [SwaggerSchema(description: "Valor total da proposta. Valor inclui o titular e todos os dependentes/agregados")]
        [SwaggerSchemaExample("50.12")]
        public decimal valorPremio { get; set; }

        [Required]
        [SwaggerSchema(description: "Data de vencimento do seguro. É a data de início de vigência do seguro")]
        [SwaggerSchemaExample("2023-01-10")]
        public DateTime dataVencimento { get; set; }
    }

    public class PropostaAnuladaDTO
    {
        [Required]
        [SwaggerSchema(description: "código da empresa")]
        [SwaggerSchemaExample("1000")]
        public int Empresa { get; set; }

        [Required]
        [SwaggerSchema(description: "código da sucursal")]
        [SwaggerSchemaExample("2000")]
        public int Sucursal { get; set; }

        [Required]
        [SwaggerSchema(description: "número da proposta que foi realizada a venda")]
        [SwaggerSchemaExample("111111")]
        public int proposta { get; set; }

        [Required]
        [SwaggerSchema(description: "unidade que realizou a venda")]
        [SwaggerSchemaExample("rio_branco, baixada, zona_norte, zona_oeste, grande_niterói, presidente_vargas, escola_de_vendas, televendas")]
        public string unidade { get; set; }

        [Required]
        [SwaggerSchema(description: "Data que a venda foi realizada")]
        [SwaggerSchemaExample("2023-01-01")]
        public DateTime dataVenda { get; set; }

        [Required]
        [SwaggerSchema(description: "Motivo da anulação da proposta")]
        [SwaggerSchemaExample("proposta rasurada")]
        public string motivo { get; set; }

        [Required]
        [SwaggerSchema(description: "Valor total da proposta. Valor inclui o titular e todos os dependentes/agregados")]
        [SwaggerSchemaExample("50.12")]
        public decimal valorPremio { get; set; }

        [Required]
        [SwaggerSchema(description: "Data de vencimento do seguro. É a data de início de vigência do seguro")]
        [SwaggerSchemaExample("2023-01-10")]
        public DateTime dataVencimento { get; set; }
    }

    public class PropostaLiberadaDTO
    {
        [SwaggerSchema(description: "código da empresa")]
        [SwaggerSchemaExample("1000")]
        public int Empresa { get; set; }

        [Required]
        [SwaggerSchema(description: "código da sucursal")]
        [SwaggerSchemaExample("2000")]
        public int Sucursal { get; set; }

        [Required]
        [SwaggerSchema(description: "número da proposta que foi realizada a venda")]
        [SwaggerSchemaExample("111111")]
        public int proposta { get; set; }

        [Required]
        [SwaggerSchema(description: "contrato de seguro que foi realizado a venda")]
        [SwaggerSchemaExample("1631")]
        public int contrato { get; set; }

        [Required]
        [SwaggerSchema(description: "unidade que realizou a venda.")]
        [SwaggerSchemaExample("rio_branco, baixada, zona_norte, zona_oeste, grande_niterói, presidente_vargas, escola_de_vendas, televendas")]
        
        public string unidade { get; set; }

        [Required]
        [SwaggerSchema(description: "Data que a venda foi realizada")]
        [SwaggerSchemaExample("2023-01-01")]
        public DateTime dataVenda { get; set; }

        [Required]
        [SwaggerSchema(description: "Valor total da proposta. Valor inclui o titular e todos os dependentes/agregados")]
        [SwaggerSchemaExample("50.12")]
        public decimal valorPremio { get; set; }

        [Required]
        [SwaggerSchema(description: "Data de vencimento do seguro. É a data de início de vigência do seguro")]
        [SwaggerSchemaExample("2023-01-10")]
        public DateTime dataVencimento { get; set; }

        [Required]
        [SwaggerSchema(description: "ID de identificação do colaborador que efetuou a venda da proposta (cdpescol)")]
        [SwaggerSchemaExample("123456")]
        public int colaborador { get; set; }

        [Required]
        [SwaggerSchema(description: "Meio de pagamento principal que o cliente escolheu para realizar o pagamento do seguro. Caso o tipo da cobrança seja debito_automatico as informações do débito automático precisam ser informadas no campo (debitoAutomatico)")]
        [SwaggerSchemaExample("cartao_credito, boleto, debito_automatico")]
        public string tipoCobranca { get; set; }

        [SwaggerSchema(description: "Informação do débito automático")]
        public DebitoAutomaticoDTO debitoAutomatico { get; set; }
    }


}