using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SEG.Core.DomainObjects
{
    public class NumeroCelular
    {
        public string Numero { get; set; }
        public static bool Validar(string numero)
        {
            if (numero == null || numero.Length < 9)
                return false;
            
            var regexCelular = new Regex(@"^[9]{1}");
            return regexCelular.IsMatch(numero);
        }
    }
}
