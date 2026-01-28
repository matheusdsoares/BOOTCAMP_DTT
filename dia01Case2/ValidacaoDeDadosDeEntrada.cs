using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dia01Case2
{
    public class ValidacaoDeDadosDeEntrada
    {
        public static void Main()
        {
            bool rodando = true;

            while (rodando)
          {
            Console.WriteLine("-- Digite Sair para finalizar o programa --");
            Console.WriteLine("--- Cadastro de Usuário ---");
            Console.Write("Digite o seu nome: ");
            string nome = Console.ReadLine();
            // Inicio da Validação de dados
            if (nome.ToLower() == "sair")
            {
                rodando = false;
                Console.WriteLine("Programa encerrado. Até logo!");
                continue; 
            }

            Console.Write("Digite a sua idade: ");
            string entradaIdade = Console.ReadLine();

            int idade = int.Parse(entradaIdade);

            Console.WriteLine("\n--- Validando Dados ---");

            Console.WriteLine("--- Iniciando Validação ---");
            
            if (string.IsNullOrWhiteSpace(nome))
            {
                Console.WriteLine("Erro: O nome não pode ser vazio.");
            }

            else if (idade <= 0)
            {
                Console.WriteLine("Erro: A idade deve ser maior que 0");
            }

            else
            {
                Console.WriteLine("Sucesso: Usuário validado com sucesso");
            }
          }  // Termino da Validação de dados
        }
    }
}