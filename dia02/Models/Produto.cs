using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dia02Produto
{
    
        public class Produto
    {
        public string Nome { get; set; }
        public double Preco { get; set; }
        public int Quantidade { get; set; }

        public Produto(string nome, double preco, int quantidade)
        {
            Nome = nome;
            Preco = preco;
            Quantidade = quantidade;
        }
    }
}
