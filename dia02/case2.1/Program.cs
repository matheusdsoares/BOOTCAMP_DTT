using System;
using System.Collections.Generic;
using System.Linq;
using case2._1;
namespace ControleVisitantesCoworking
{


    class Program
    {
        // Uso de Collection (List) para armazenar os visitantes
        static List<Visitante> listaVisitantes = new List<Visitante>();
        static int proximoId = 1;

        static void Main(string[] args)
        {
            bool executando = true;

            while (executando)
            {
                try 
                {
                    Console.Clear();
                    Console.WriteLine("=== SISTEMA DE CHECK-IN COWORKING ===");
                    Console.WriteLine("1. Cadastrar Visitante");
                    Console.WriteLine("2. Listar Visitantes");
                    Console.WriteLine("3. Buscar Visitante por Nome");
                    Console.WriteLine("4. Registrar Saída");
                    Console.WriteLine("5. Listar Apenas Primeira Visita");
                    Console.WriteLine("0. Sair");
                    Console.Write("Opção: ");

                    
                    switch (Console.ReadLine())
                    {
                        case "1": Cadastrar(); break;
                        case "2": Listar(); break;
                        case "3": Buscar(); break;
                        case "4": RegistrarSaida(); break;
                        case "5": FiltrarPrimeiraVez(); break;
                        case "0": executando = false; break;
                        default: Console.WriteLine("Opção inválida!"); break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro inesperado: {ex.Message}");
                }

                if (executando)
                {
                    Console.WriteLine("\nPressione qualquer tecla para continuar...");
                    Console.ReadKey();
                }
            }
        }

        static void Cadastrar()
        {
            try
            {
                Console.Write("Nome: ");
                string nome = Console.ReadLine();

                Console.Write("Documento: ");
                string doc = Console.ReadLine();

                Console.Write("É a primeira vez? (S/N): ");
                bool primeira = Console.ReadLine().ToUpper() == "S";

                Visitante novo = new Visitante
                {
                    Id = proximoId++,
                    Nome = nome,
                    Documento = doc,
                    HorarioChegada = DateTime.Now,
                    IsPrimeiraVez = primeira
                };

                listaVisitantes.Add(novo);
                Console.WriteLine("Visitante cadastrado com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao cadastrar: {ex.Message}");
            }
        }

        static void Listar()
        {
            Console.WriteLine("\n--- Lista de Visitantes (Ordenada por ID) ---");
            // Desafio Extra: Ordenar por ID usando LINQ
            var listaOrdenada = listaVisitantes.OrderBy(v => v.Id).ToList();

            if (listaOrdenada.Count == 0) Console.WriteLine("Nenhum visitante registrado.");
            foreach (var v in listaOrdenada) Console.WriteLine(v);
        }

        static void Buscar()
        {
            Console.Write("Digite o nome para busca: ");
            string busca = Console.ReadLine().ToLower();

            var resultados = listaVisitantes.Where(v => v.Nome.ToLower().Contains(busca)).ToList();

            if (resultados.Any())
                foreach (var v in resultados) Console.WriteLine(v);
            else
                Console.WriteLine("Nenhum visitante encontrado.");
        }

        static void RegistrarSaida()
        {
            Console.Write("Digite o ID do visitante para saída: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var v = listaVisitantes.FirstOrDefault(vis => vis.Id == id);
                if (v != null)
                {
                    v.HorarioSaida = DateTime.Now;
                    Console.WriteLine($"Saída de {v.Nome} registrada às {v.HorarioSaida:HH:mm}.");
                }
                else Console.WriteLine("ID não encontrado.");
            }
            else Console.WriteLine("ID inválido.");
        }

        static void FiltrarPrimeiraVez()
        {
            Console.WriteLine("\n--- Visitantes em sua Primeira Vez ---");
            // Desafio Extra: Filtrar apenas primeira visita
            var apenasPrimeira = listaVisitantes.Where(v => v.IsPrimeiraVez).ToList();

            if (apenasPrimeira.Count == 0) Console.WriteLine("Nenhum visitante na primeira vez.");
            foreach (var v in apenasPrimeira) Console.WriteLine(v);
        }
    }
}
