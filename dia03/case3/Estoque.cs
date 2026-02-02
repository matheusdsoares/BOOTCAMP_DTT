class Estoque
{
    int id;
    decimal producaoId;
    decimal quantidade;
    string local;

    //getters e setters

    //SOLID
    //KISS
    
    public int montarEstoque(int id, int producaoId, decimal quantidade, string local)
    {
        bool isIdValido = (id < 0);
        bool isProdValida = (producaoId < 0);
        bool idQuantidadeValida = (quantidade > 1);

        bool isRegraNegocioFinalOK = (isIdValido && isProdValida);



        if ((isRegraNegocioFinalOK))
        {
            return 0;
        }
        else
        {
            return 1;
        }
    }
}