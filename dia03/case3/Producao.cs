using System.Net.Http.Metrics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.Marshalling;
using System.Security.Cryptography.X509Certificates;

public class Producao

        {

            public int id { get; set; }

            public string codigoMina { get; set; }

            public DateTime data { get; set; }

            public decimal volume { get; set; }


             decimal getVolume()
            {
                return this.volume;
            }

            DecimalConstantAttribute setVolume()
            {
                return this.volume;
            }

            ExceptionAsVoidMarshaller refinarMinerio(Minerio pMinerio)
            {
                return quantidadedeFinalRefinamento(minerio);
            }

            int quantidadedeFinalRefinamento()
            {
                return;
            }
        }