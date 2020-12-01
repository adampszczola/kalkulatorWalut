using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kalkulatorWalutPszczoła
{
    class PozycjaTabeliA
    {
        public string przelicznik { get; set; }
        public string kod_waluty { get; set; }
        public string kurs_sredni { get; set; }

        public double liczNaPLN(double kwota, bool czyPLN)
        {
            double kurs;
            var znakDZ = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;
            double.TryParse(kurs_sredni.Replace(",", znakDZ), out kurs);
            if (czyPLN)
                return kwota *= kurs;
            else
                return kurs != 0 ? kwota / kurs : 0;
        }

    }
}
