class Producao
{
    private int id;
    private string codigoMina;
    private DateTime data;
    private decimal volume;

    decimal getVolume()
    {
        return this.volume;
    }

    decimal setVolume()
    {
        return this.volume;
    }

    //getters e setters

    public int refinarMinerio(Minerio pMinerio, Refinamento refinamento)
    {
        switch (refinamento) {
            case Refinamento.Granularidade:
                return 0;
        }
            
        return this.quantidadeFinalRefinamento(pMinerio);
    }

    private int quantidadeFinalRefinamento(Minerio pMinerio) {
        return 1;
    }
}