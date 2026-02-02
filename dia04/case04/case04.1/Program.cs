using System.ComponentModel;
using Banco;

class Program
{
    static void Main()
    {
        var contaUsuario = new ContaCorrente("0500-x", 500, false, 0 );
        Console.WriteLine("Conta do Usuario");
        Console.WriteLine(contaUsuario);

        Console.WriteLine("Tentando sacar R$ 600,00 (de falhar)");
        bool sacou = contaUsuario.sacar(400);
        Console.WriteLine($"Saque realizado? {sacou ? "sim" : "nao"}. Saldo: {consultarSaldo.contaUsuario}")

























        // // 1. Criando uma conta com: Numero, Saldo Inicial, Se é Especial, e Limite
        // ContaCorrente minhaConta = new ContaCorrente("101", 500, true, 1000);

        // Console.WriteLine("--- Início dos Testes ---");

        // // 2. Consultando o saldo inicial
        // minhaConta.consultarSaldo();

        // // 3. Testando um saque que entra no cheque especial
        // // Saldo é 500, vamos sacar 800. Ele vai ficar com -300.
        // minhaConta.sacar(550);

        // minhaConta.consultarSaldo();
        
        // // 4. Chamando o método que você criou para verificar o cheque especial
        // minhaConta.verificarChequeEspecial();

        // minhaConta.consultarSaldo();
        // // 5. Fazendo um depósito para sair do cheque especial
        // minhaConta.depositar(500);

        
        // minhaConta.consultarSaldo();
        // minhaConta.verificarChequeEspecial();

        // Console.WriteLine("--- Fim dos Testes ---");
    }
}
