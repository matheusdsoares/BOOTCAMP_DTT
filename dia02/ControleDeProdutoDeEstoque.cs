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

using System;
using System.Collections.Generic;

using System;
using System.Collections.Generic;

using System;
using System.Collections.Generic;
using System.Linq;

namespace dia02
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

    public class ControleDeProdutoDeEstoque
    {
        public static void Main()
        {
            List<Produto> listaProdutos = new List<Produto>();
            string continuar;

            // --- FASE 1: CADASTRO ---
            do
            {
                try
                {
                    Console.Write("\nNome do produto: ");
                    string nome = Console.ReadLine();
                    Console.Write("Preço: ");
                    double preco = double.Parse(Console.ReadLine());
                    Console.Write("Quantidade: ");
                    int qtd = int.Parse(Console.ReadLine());

                    if (string.IsNullOrWhiteSpace(nome) || preco <= 0 || qtd <= 0)
                        Console.WriteLine("Erro: Dados inválidos (Preço > 0 e Qtd > 0).");
                    else
                    {
                        listaProdutos.Add(new Produto(nome, preco, qtd));
                        Console.WriteLine("Produto cadastrado!");
                    }
                }
                catch { Console.WriteLine("Erro: Entrada inválida."); }

                Console.Write("Cadastrar outro produto? (s/n): ");
                continuar = Console.ReadLine().ToLower();
            } while (continuar == "s");

            ExibirRelatorio(listaProdutos, "ESTOQUE ATUAL");

            // --- FASE 2: EXCLUSÃO REPETITIVA ---
            if (listaProdutos.Count > 0)
            {
                do
                {
                    Console.Write("\nNome do produto para REMOVER (ou Enter para pular): ");
                    string busca = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(busca)) break;

                    int removidos = listaProdutos.RemoveAll(p => p.Nome.Equals(busca, StringComparison.OrdinalIgnoreCase));
                    
                    if (removidos > 0) Console.WriteLine("Removido com sucesso.");
                    else Console.WriteLine("Produto não encontrado.");

                    if (listaProdutos.Count == 0) break;

                    Console.Write("Deseja remover outro? (s/n): ");
                    continuar = Console.ReadLine().ToLower();
                } while (continuar == "s");
            }

            // --- FASE 3: EDIÇÃO REPETITIVA ---
            if (listaProdutos.Count > 0)
            {
                do
                {
                    Console.Write("\nNome do produto para EDITAR (ou Enter para pular): ");
                    string busca = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(busca)) break;

                    Produto p = listaProdutos.Find(x => x.Nome.Equals(busca, StringComparison.OrdinalIgnoreCase));

                    if (p != null)
                    {
                        try
                        {
                            Console.Write($"Novo preço para {p.Nome} (Atual: {p.Preco:F2}): ");
                            p.Preco = double.Parse(Console.ReadLine());
                            Console.Write($"Nova quantidade (Atual: {p.Quantidade}): ");
                            int novaQtd = int.Parse(Console.ReadLine());
                            
                            if (novaQtd > 0) p.Quantidade = novaQtd;
                            else Console.WriteLine("Quantidade precisa ser maior que 0.");
                            
                            Console.WriteLine("Dados atualizados!");
                        }
                        catch { Console.WriteLine("Erro nos dados. Edição cancelada."); }
                    }
                    else Console.WriteLine("Produto não encontrado.");

                    Console.Write("Deseja editar outro? (s/n): ");
                    continuar = Console.ReadLine().ToLower();
                } while (continuar == "s");
            }

            // --- FASE FINAL ---
            ExibirRelatorio(listaProdutos, "RELATÓRIO FINAL");
            Console.WriteLine("\nFim do programa. Pressione qualquer tecla...");
            Console.ReadKey();
        }

        public static void ExibirRelatorio(List<Produto> lista, string titulo)
        {
            Console.WriteLine($"\n--- {titulo} ---");
            if (lista.Count == 0) Console.WriteLine("Estoque vazio.");
            else
            {
                Console.WriteLine($"{"Nome",-15} | {"Preço",-10} | {"Qtd",-5}");
                lista.ForEach(p => Console.WriteLine($"{p.Nome,-15} | R$ {p.Preco,-7:F2} | {p.Quantidade,-5}"));
            }
        }
    }
}