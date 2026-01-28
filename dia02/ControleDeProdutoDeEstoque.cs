using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

using System;
using System.Collections.Generic;

namespace dia02
{
    public class Produto
    {
        
        public string Nome { get; private set; }
        public double Preco { get; private set; }

        
        public Produto(string nome, double preco)
        {
            Nome = nome;
            Preco = preco;
        }
    }

    public class ControleDeProdutoDeEstoque
    {
        public static void Main()
        {
            
            List<Produto> listaProdutos = new List<Produto>();
            string continuar = "s";

            while (continuar.ToLower() == "s")
            {
                Console.Write("Digite o nome do produto: ");
                string nomeInput = Console.ReadLine();

                Console.Write("Digite o preço do produto: ");
                double precoInput = double.Parse(Console.ReadLine());

                // Validação
                if (string.IsNullOrWhiteSpace(nomeInput))
                {
                    Console.WriteLine("Erro: O nome não pode ser vazio.");
                }
                else if (precoInput <= 0)
                {
                    Console.WriteLine("Erro: O preço deve ser maior que 0.");
                }
                else
                {
                    
                    Produto novoProduto = new Produto(nomeInput, precoInput);
                    listaProdutos.Add(novoProduto);
                    Console.WriteLine("Produto cadastrado com sucesso!");
                }

                Console.Write("\nDeseja cadastrar outro? (s/n): ");
                continuar = Console.ReadLine();
            }

            // Exibindo a lista final
            Console.WriteLine("\n--- RELATÓRIO DE ESTOQUE ---");
            foreach (var prod in listaProdutos)
            {
                // Aqui usamos o 'get' das propriedades Nome e Preco
                Console.WriteLine($"Produto: {prod.Nome} - Preço: R$ {prod.Preco:F2}");
            }
        }
    }
}