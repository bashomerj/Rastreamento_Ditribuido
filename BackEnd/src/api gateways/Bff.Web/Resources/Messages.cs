﻿using System.ComponentModel;

namespace Bff.Web.Resources
{
    public enum Messages
    {
        //[Description("teste")]
        contrato_nao_encontrato,
        contrato_nao_vida_individual,
        contrato_nao_habilitado_comercializacao,
        contrato_nao_pertence_empresa_sucursal,
        contrato_inativo,
        contrato_nao_vigente,
        proposta_nao_preenchida,
        contrato_nao_preenchido,
        sucursal_nao_preenchida,
        proposta_nao_liberada_comercializacao,
        endereco_cobranca_nao_informado,
        uf_nao__habilitada_venda,
        agrupamento_nao_encontrado,
        agrupamento_inativo,
        data_nascimento_menor_data_base,
        idade_fora_limite_aceitacao,
        titular_cobertura_nao_informada,
        cobertura_is_negativo,
        motivo_venda_administrativa_nao_encontrato,
        certificado_original_venda_administrativa_nao_informado,
        contrato_original_venda_administrativa_não_informado,
        contrato_original_venda_administrativa_não_encontrato,
        certificado_original_venda_administrativa_nao_encontrato,
        certificado_original_venda_administrativa_nao_cancelado,
        certificado_original_venda_administrativa_nao_pertence_titular,
        colaborador_nao_informado,
        colaborador_nao_encontrado,
        colaborador_inativo,
        empresa_nao_informada,
        proposta_processada_anteriormente,
        proposta_recusada_nao_marcada_redigitacao,
        cpf_igual_segurados,
        estado_civil_obrigadorio,
        nome_nao_informado,
        nome_fora_limite_caracter,
        nome_caracter_especial,
        data_nascimento_nao_informada,
        data_nascimento_maior_data_atual,
        data_nascimento_maior_vigencia_seguro,
        email_invalido,
        renda_menor_zero,
        cpf_cadastrado_como_proprio_existente,
        rg_cadastrado_para_outro_segurado,
        celular_whatsapp_marcado_somente_um,
        celular_principal_marcado_somente_um,
        telefone_marcado_whatsapp_nao_celular,
        celular_mais_do_que_um_informado_como_principal,
        ddd_nao_informado,
        ddd_invalido,
        telefone_nao_informado,
        celular_invalido,
        telefone_marcado_principal_nao_celular,
        telefone_marcado_sms_nao_celular,
        telefone_nao_celuluar_marcado_principal_,
        telefone_nao_celuluar_marcado_whatsapp,
        telefone_nao_celuluar_principal_marcado_whatsapp,
        rg_nao_informado,
        orgao_expedidor_rg_nao_informado,
        orgao_expedidor_rg_caracter_especial,
        data_expedicao_rg_nao_informada,
        data_expedicao_rg_menor_nascimento,
        data_expedicao_rg_menor_data_atual,
        titular_cpf_proprio_obrigatorio,
        cpf_não_informado,
        cpf_invalido,
        endereco_não_informado,
        cep_não_informado,
        logradouro_não_informado,
        endereco_caracter_especial,
        cidade_não_informado,
        cidade_caracter_especial,
        uf_não_informado,
        uf_caracter_especial,
        complemento_caracter_especial,
        referencia_caracter_especial,
        cpf_vinculado_colaborador_ativo,
        cpf_outra_pessoa_cadastrada,
        percentual_participacao_não_informado,
        percentual_participacao_menor_igual_zero,
        percentual_participacao_maior_cem,
        nome_beneficiario_não_informado,
        nome_beneficiario_limite_caracter,
        nome_beneficiario_caracter_especial,
        sexo_beneficiario_não_informado,
        data_nascimento_beneficiario_não_informada,
        data_nascimento_beneficiario_maior_data_atual,
        tipo_telefone_não_informado,
        produto_plano_não_informado,
        sequencial_plano_não_informado,
        vigencia_plano_não_informada,
        cobertura_contratada_não_informada,
        codigo_cobertura_não_informado,
        is_cobertura_negativo,
        plano_informado_não_encontrato,
        plano_informado_não_disponivel_tipo_segurado,
        plano_informado_não_disponivel_sexo_segurado,
        cobertura_não_informada,
        sem_cobertura_parametrizada_para_plano,
        cobertura_informada_não_conta_plano,
        cobertura_basica_não_informada_ou_zerada,
        cobertura_adicional_não_informada_ou_zerada,
        cobertura_especial_não_informada_ou_zerada,
        plano_sem_cobertura_basica,
        cobertura_não_parametrizada_plano,
        coberuta_não_comercializada_tipo_segurado,
        cobertura_fora_limite_is_para_faixa_etaria,
        cobertura_divergencia_valor_informado_valor_calculado,
        dps_não_parametrizada_para_produto,
        dps_nehuma_pergunta_respondida,
        dps_pergunta_sem_resposta,
        dps_pergunta_duplicada,
        dps_pergunta_faltando_dps_enviada,
        dps_resposta_pergunta_invalida,
        dps_resposta_pergunta_não_permite_complemento,
        dps_pergunta_não_parametrizada_produto,
        meio_cobranca_não_informada,
        proposta_meio_pagamento_não_informada,
        REMOVER,
        agencia_conta_debito_não_infomada,
        digito_agencia_conta_debito_não_informada,
        conta_debito_não_informada,
        digito_conta_debito_não_informada,
        tipo_conta_debito_não_informado,
        categoria_conta_debito_não_informada,
        nome_titular_conta_debito_não_informado,
        cpf_titular_conta_debito_não_informado,
        cpf_titular_conta_debito_invalido

    }

    public static class DigitalExtensions
    {
        public static string ToDescriptionString(this Messages val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val
               .GetType()
               .GetField(val.ToString())
               .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }

    }
}
