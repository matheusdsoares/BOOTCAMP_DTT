using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;

namespace Banco
{
    public class ContaCorrente
    {
        public string Numero {get; set;}
        public decimal Saldo {get; private set;}
        public bool IsEspecial {get;}

        public decimal Limite {get;}


        public ContaCorrente(string numero, decimal saldo, bool isEspecial, decimal limite)
        {
            if (string.IsNullOrWhiteSpace(numero))
            {
                throw new ArgumentException("Número da conta é obrigatorio.", nameof(numero));
            }

            if (limite < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(limite), "limite não pode ser negativo");
            }

            Numero = numero;
            Saldo = saldo;
            IsEspecial = isEspecial;
            Limite = limite;

        }

        public void sacar(decimal valor)
        {
            if(valor >(Saldo + Limite))
            {
                Console.WriteLine($"Saque de R${valor} negado, devido a não ter saldo suficiente");
            }
            if(!IsEspecial)
            {
                if(valor <= Saldo)
                {
                    Saldo = valor;
                    return true;
                }
                return false;
            }
            
            // else
            // {
            //     Saldo -= valor;
            //     Console.WriteLine($"Saque de R${valor} realizado com sucesso.");
            // }
        }

        public void depositar(decimal valor)
        {
            if (valor <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(valor), "O valor do deposito deve ser positivo");
            }
            // Saldo = valor;
            // Saldo += valor;
            // Console.WriteLine($"Valor R${valor} depositado");
        }

        public decimal consultarSaldo() => Saldo;
        // {
        //     Console.WriteLine($"Seu saldo é de R${Saldo}");
        // }

        public bool estaUsandoChequeEspecial() => Saldo < 0;

        public override string ToString()
        {
            return $"Conta {Numero} | Saldo: {Saldo:F2} | Especial: {(IsEspecial ? "Sim" : "Não")} "
        }

        // public void verificarChequeEspecial()
        // {
        //     if(Saldo < 0)
        //     {
        //         Console.WriteLine("O cliente está usando o cheque especial");
        //     }
        //     else
        //     {
        //         Console.WriteLine("O cliente não está usando o cheque especial.");
        //     }
        // }

        

    }
}