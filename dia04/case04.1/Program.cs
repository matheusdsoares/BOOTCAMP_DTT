using ContaBancaria;

class Program
{
    static void Main()
    {
        ContaCorrente minhaConta = new ContaCorrente(1, 50, false, 0);

        minhaConta.ConsultarSaldo();
        minhaConta.Saque(50.00);
        minhaConta.Saque(100.00);
        minhaConta.ConsultarSaldo();
        minhaConta.VerificarUsoChequeEspecial();
        minhaConta.Depositar(200.00);
        minhaConta.ConsultarSaldo();
    }
}
