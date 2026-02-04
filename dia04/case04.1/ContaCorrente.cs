using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;

namespace ContaBancaria
{
    public class ContaCorrente
    {
        public int Numero {get; set;}
        public double Saldo {get; private set;}
        public bool EhEspecial {get; set;}
        public double Limite {get; set;}
        public ContaCorrente(int numero, double saldoInicial, bool ehEspecial, double limite)
        {
            Numero = numero;
            Saldo = saldoInicial;
            EhEspecial = ehEspecial;
            Limite = limite;
        }

        public void Saque(double valor)
        {
            if(valor > (Saldo + Limite))
            {
                Console.WriteLine($"Saque de R${valor} negado, saldo insuficiente");
            }
            else
            {
                Saldo -= valor;
                Console.WriteLine($"Saque de R${valor} realizado com sucesso");

            }
        }

        public void Depositar(double valor)
        {
            Saldo += valor;
            Console.WriteLine($"Seu valor de R${valor} foi depositado");
        }

        public void ConsultarSaldo()
        {
            Console.WriteLine($"Saldo atual da conta {Numero}: R${Saldo}");
        }

        public void VerificarUsoChequeEspecial()
        {
            if (Saldo < 0)
            {
                Console.WriteLine("Atenção: Você está utilizando o Cheque Especial");
            }
            else
            {
                Console.WriteLine("Você não está utilizando o cheque especial.");
            }
        }
    }
}