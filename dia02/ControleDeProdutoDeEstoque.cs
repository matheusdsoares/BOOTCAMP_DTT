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
            string continuar = "s";

            while (continuar.ToLower() == "s")
            {
                try
                {
                    Console.Write("\nDigite o nome do produto: ");
                    string nomeInput = Console.ReadLine();

                    Console.Write("Digite o preço do produto: ");
                    double precoInput = double.Parse(Console.ReadLine());

                    Console.Write("Digite a quantidade inicial: ");
                    int qtdInput = int.Parse(Console.ReadLine());

                    // Validação de entrada
                    if (string.IsNullOrWhiteSpace(nomeInput) || precoInput <= 0 || qtdInput <= 0)
                    {
                        Console.WriteLine("Erro: Verifique os dados, nome não pode ser nulo, nem conter espaços. (Preço > 0 e Quantidade > 0).");
                    }
                    else
                    {
                        listaProdutos.Add(new Produto(nomeInput, precoInput, qtdInput));
                        Console.WriteLine("Produto cadastrado com sucesso!");
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine("Erro: Use apenas números nos campos de preço e quantidade.");
                }

                Console.Write("Deseja cadastrar outro? (s/n): ");
                continuar = Console.ReadLine();
            }

            ExibirRelatorio(listaProdutos, "RELATÓRIO DE ESTOQUE");

            // --- REMOÇÃO ---
            if (listaProdutos.Count > 0)
            {
                Console.Write("\nDigite o nome do produto que deseja REMOVER (ou Enter para pular): ");
                string nomeParaRemover = Console.ReadLine();
                int removidos = listaProdutos.RemoveAll(p => p.Nome.Equals(nomeParaRemover, StringComparison.OrdinalIgnoreCase));
                if (removidos > 0) Console.WriteLine("Produto removido com sucesso!");
            }

            // --- ATUALIZAÇÃO (PREÇO E QUANTIDADE) ---
            if (listaProdutos.Count > 0)
            {
                Console.Write("\nDigite o nome do produto para EDITAR: ");
                string nomeParaEditar = Console.ReadLine();
                Produto prodEncontrado = listaProdutos.Find(p => p.Nome.Equals(nomeParaEditar, StringComparison.OrdinalIgnoreCase));

                if (prodEncontrado != null)
                {
                    try
                    {
                        Console.WriteLine($"\nEditando: {prodEncontrado.Nome}");
                        Console.Write("Novo preço (ou digite o atual): ");
                        prodEncontrado.Preco = double.Parse(Console.ReadLine());

                        Console.Write("Nova quantidade (não pode ser menor ou igual a 0): ");
                        int novaQtd = int.Parse(Console.ReadLine());

                        if (novaQtd <= 0)
                        {
                            Console.WriteLine("Erro: Quantidade inválida. A alteração de quantidade foi ignorada.");
                        }
                        else
                        {
                            prodEncontrado.Quantidade = novaQtd;
                            Console.WriteLine("Dados atualizados com sucesso!");
                        }
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Erro: Entrada inválida. Operação cancelada.");
                    }
                }
                else if (!string.IsNullOrWhiteSpace(nomeParaEditar))
                {
                    Console.WriteLine("Produto não encontrado.");
                }
            }

            // --- EXIBIÇÃO FINAL ---
            ExibirRelatorio(listaProdutos, "RELATÓRIO FINAL DE ESTOQUE");

            Console.WriteLine("\nPrograma finalizado. Pressione qualquer tecla para sair...");
            Console.ReadKey();
        }

        public static void ExibirRelatorio(List<Produto> lista, string titulo)
        {
            Console.WriteLine($"\n--- {titulo} ---");
            if (lista.Count == 0)
            {
                Console.WriteLine("O estoque está vazio.");
            }
            else
            {
                
                Console.WriteLine($"{"Nome",-15} | {"Preço",-10} | {"Qtd",-5}");
                Console.WriteLine(new string('-', 35));
                foreach (var prod in lista)
                {
                    Console.WriteLine($"{prod.Nome,-15} | R$ {prod.Preco,-7:F2} | {prod.Quantidade,-5}");
                }
            }
        }
    }
}