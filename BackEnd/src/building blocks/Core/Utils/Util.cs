using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Utils
{
    public static class Util
    {
        public static int CalcularIdade(DateTime dataBase, DateTime dataNascimento)
            {
                if (dataBase == null || dataNascimento == null)
                    return -1;

                if (dataBase.Year < dataNascimento.Year)
                    return -1;

                var idade = dataBase.Year - dataNascimento.Year;

                if (dataNascimento.Month > dataBase.Month)
                    return --idade;

                if (dataNascimento.Month < dataBase.Month)
                    return idade;

                if (dataNascimento.Month == dataBase.Month)
                    if (dataNascimento.Day > dataBase.Day)
                        return --idade;

                return idade;

            }

        public static DateTime CalcularFimDaVigencia(DateTime data)
        {
            return data.AddYears(1);
        }

        //public static int TraduzTipoSegurado(string TipoSegurado)
        //{
        //    switch (TipoSegurado.ToUpper())
        //    {
        //        case "TITULAR":
        //            return 0;
        //        case "CONJUGE":
        //            return 4;
        //        case "FILHO":
        //            return 3;
        //        case "AGREGADO":
        //            return 1;
        //    }

        //    return -1;
        //}

        //public static int TraduzSexo(string sexo)
        //{
        //    switch (sexo.ToLower())
        //    {
        //        case "masculino":
        //            return 0;
        //        case "feminino":
        //            return 1;
        //    }

        //    return -1;
        //}

       
        
    }
}
