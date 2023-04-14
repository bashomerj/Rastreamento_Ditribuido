using System;
using System.Collections.Generic;
using System.Text;

namespace SEG.Core.DomainObjects
{
    public class AnoBissexto
    {
        public DateTime Data { get; set; }

        public static bool Validar(DateTime data)
        {
            if (data == null)
            {
                return false;
            }

            int year = data.Year;

            if (data.Month == 2)
            {
                return ((year % 4 == 0) && (year % 100 != 0)) || (year % 400 == 0);
            }
            return true;
        }
    }
}
