using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Minerio01;
namespace Mina
{
    public static void Main()
    {
        Mina mina = new Mina();
        mina.acessarExtrairMinerio;
    }
    public class Mina
    {
        private Minerio? minerio;
        private string? codigo;
        private string? nome;
        private decimal? capacidade;

        public Minerio getMinerio()
        {
            return this.minerio;
        }

        public void setMinerio(Minerio pMinerio)
        {

        }

        public string getCodigo()
        {
            return this.codigo;
        }

        public void setCodigo(string pCodigo)
        {

        }

        public string getNome()
        {
            return this.nome;
        }

        public void setNome(string pNome)
        {

        }

        public decimal getCapacidade()
        {
            return (decimal)this.capacidade;
        }

        public void setCapacidade(decimal pCapacidade)
        {

        }

        private Minerio extrairMinerio()
        {
            Minerio minerio = new Minerio();
            minerio.codigo = "1";
            minerio.tipo = "Ouro";

            return minerio;
        }

        public Minerio acessarExtrairMinerio(bool isGestorMina)
        {
            if (isGestorMina)
            {
                return this.extrairMinerio();
            }
            else
            {
                Minerio minerio = new Minerio();
                minerio.codigo = "0";
                return minerio;
            }
        }
    }
}