using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UsodaLampada
{
    public class Lampada
    {
        private bool ligada;

        public Lampada()
            {
                ligada = false;
            }

        public void ligar()
        {
            ligada = true;
            Console.WriteLine("Lâmpada ligada");
        }

        public void desligar()
        {
            ligada = false;
            Console.WriteLine("Lâmpada desligada");
        }

        public bool estaLigada()
        {
            return this.ligada;
        }        
    }    
}