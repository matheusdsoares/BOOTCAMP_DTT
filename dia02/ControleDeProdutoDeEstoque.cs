using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

using System;
using System.Collections.Generic;

using System;
using System.Collections.Generic;

using System;
using System.Collections.Generic;

namespace dia02
{
    public class Produto
    {
        public string Nome { get; set; }
        public double Preco { get; set; }

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
                try
                {
                    Console.Write("\nDigite o nome do produto: ");
                    string nomeInput = Console.ReadLine();

                    Console.Write("Digite o preço do produto: ");
                    double precoInput = double.Parse(Console.ReadLine());

                    if (string.IsNullOrWhiteSpace(nomeInput) || precoInput <= 0)
                    {
                        Console.WriteLine("Erro: Nome vazio ou preço inválido.");
                    }
                    else
                    {
                        listaProdutos.Add(new Produto(nomeInput, precoInput));
                        Console.WriteLine("Produto cadastrado com sucesso!");
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine("Erro: No campo preço, use apenas números e vírgula.");
                }

                Console.Write("Deseja cadastrar outro? (s/n): ");
                continuar = Console.ReadLine();
            }

            Console.WriteLine("\n--- RELATÓRIO DE ESTOQUE ---");
            foreach (var prod in listaProdutos)
            {
                // Aqui usamos o 'get' das propriedades Nome e Preco
                Console.WriteLine($"Produto: {prod.Nome} - Preço: R$ {prod.Preco:F2}");
            }
            
            if (listaProdutos.Count > 0)
            {
                Console.Write("\nDigite o nome do produto que deseja REMOVER (ou Enter para pular): ");
                string nomeParaRemover = Console.ReadLine();
                
                int removidos = listaProdutos.RemoveAll(p => p.Nome.Equals(nomeParaRemover, StringComparison.OrdinalIgnoreCase));
                
                if (removidos > 0) Console.WriteLine("Produto removido com sucesso!");
            }

            
            if (listaProdutos.Count > 0)
            {
                Console.Write("\nDigite o nome do produto para ATUALIZAR o preço: ");
                string nomeParaEditar = Console.ReadLine();
                Produto prodEncontrado = listaProdutos.Find(p => p.Nome.Equals(nomeParaEditar, StringComparison.OrdinalIgnoreCase));

                if (prodEncontrado != null)
                {
                    try
                    {
                        Console.Write($"Digite o novo preço para {prodEncontrado.Nome}: ");
                        prodEncontrado.Preco = double.Parse(Console.ReadLine());
                        Console.WriteLine("Preço atualizado!");
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Erro: Valor inválido. A atualização foi cancelada.");
                    }
                }
                else if (!string.IsNullOrWhiteSpace(nomeParaEditar))
                {
                    Console.WriteLine("Produto não encontrado.");
                }
            }

            // --- EXIBIÇÃO FINAL ---
            Console.WriteLine("\n--- RELATÓRIO FINAL DE ESTOQUE ---");
            if (listaProdutos.Count == 0)
            {
                Console.WriteLine("O estoque está vazio.");
            }
            else
            {
                foreach (var prod in listaProdutos)
                {
                    Console.WriteLine($"Produto: {prod.Nome.PadRight(15)} | Preço: R$ {prod.Preco:F2}");
                }
            }

            Console.WriteLine("\nPrograma finalizado. Pressione qualquer tecla para sair...");
            Console.ReadKey();
        }
    }
}