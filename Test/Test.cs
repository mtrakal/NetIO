using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test
{
    class Test
    {
        public string Vstup { get; set; }
        public string Kontrola { get; set; }

        public Test()
        {

        }
        public Test(string vstup, string kontrola)
        {
            Vstup = vstup;
            Kontrola = kontrola;
        }
        public bool Prosel(string prijataData)
        {
            if (Kontrola == "DATAINFO")
            {
                return (prijataData.Trim().Length != 0) ? true : false;
            }
            if (Kontrola == "DATANULL")
            {
                return (prijataData.Trim().Length == 0) ? true : false;
            }
            return Kontrola == prijataData.Trim();
        }
        public override string ToString()
        {
            return Vstup + " (" + Kontrola + ")";
        }
    }
}
