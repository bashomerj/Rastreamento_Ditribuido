using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Core.DomainObjects
{
    public class CaracterEspecial
    {
        public string Caracter { get; set; }

        public static bool NaoPermitir(string caracter)
        {
            //TODO: Validar a expressão regular pois não está funcionando

            if (caracter == null)
                return true;

            var regexEspecial = new Regex(@"[^a-zA-Z0-9][.]+");
            return !regexEspecial.IsMatch(caracter);

        }
    }
}
