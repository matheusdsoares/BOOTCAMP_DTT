using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ItemDaCasa
{
    public class Lampada
    {
        private bool isLigada;

        public Lampada()
        {
            isLigada = false;
        }

        public void Ligar()
        {
            isLigada = true;
            Console.WriteLine("Lampada ligada");
        }

        public void Desligar()
        {
            isLigada = false;
            Console.WriteLine("Lampada desligada");
        }

        public bool VerificarLampada()
        {
            return isLigada;
        }
    }
}