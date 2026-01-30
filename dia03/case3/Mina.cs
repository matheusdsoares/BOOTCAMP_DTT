using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

public class Mina

        {

            private Minerio? minerio;

            private string? codigo;

            private string? nome;

            private decimal? capacidade; 




            public string getCodigo()
            {
                return this.codigo;
            }

            public void setCodigo(string pCodigo)
            {
                
            }

            public string getnome()
            {
                return this.nome;
            }

            public void setNome(string pNome)
            {
                
            }

            public string getMinerio()
            {
                return this.minerio;
            }

            public void setMinerio(string pMinerio)
            {
                
            }

            public string getCapacidade()
            {
                return (decimal) this.capacidade;
            }

            public void setCapacidade(string pCodigo)
            {
                
            }


            public Minerio acessarExtrairMinerio(bool isGestorMina)
            {
                if (isGestorMina)
                {
                    return this.extrairMinerio();
                } else
                {
                    Minerio minerio = new Minerio();
                    minerio.codigo = "0";
                    return minerio;
                }
            }

            public Minerio extrairMinerio()

            {
                Minerio minerio = new Minerio();
                minerio.codigo = "1";
                minerio.tipo = "Ouro";

                return minerio;

            }

        }


