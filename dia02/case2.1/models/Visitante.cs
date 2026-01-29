using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace case2._1
{
    // Classe Visitante com Properties (get/set)
    public class Visitante
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Documento { get; set; }
        public DateTime HorarioChegada { get; set; }
        public DateTime? HorarioSaida { get; set; } // Nullable para visitantes ainda presentes
        public bool IsPrimeiraVez { get; set; }

        public override string ToString()
        {
            string saida = HorarioSaida.HasValue ? HorarioSaida.Value.ToString("HH:mm") : "Ainda presente";
            return $"[ID: {Id}] {Nome} - Doc: {Documento} | Chegada: {HorarioChegada:HH:mm} | Saída: {saida} | Primeira Vez: {(IsPrimeiraVez ? "Sim" : "Não")}";
        }
    }
}