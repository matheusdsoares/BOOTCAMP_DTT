using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SuaApi.Models;

public enum TipoEquipamento
{
    Caminhao, Escavadeira, Perfuratriz, Carregadeira, Trator
}

public enum StatusOperacional
{
    Operacional, EmManutencao, Parado
}

public class Equipamento
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "O código é obrigatório.")]
    [StringLength(50)]
    public string Codigo { get; set; } = string.Empty;

    [Required]
    public TipoEquipamento Tipo { get; set; }

    [Required]
    [StringLength(100)]
    public string Modelo { get; set; } = string.Empty;

    [Range(0, double.MaxValue, ErrorMessage = "O horímetro não pode ser negativo.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Horimetro { get; set; }

    [Required]
    public StatusOperacional StatusOperacional { get; set; }

    [Required]
    // Força o tipo 'date' no Postgres (opcional, se quiser ignorar a hora)
    [Column(TypeName = "date")] 
    public DateTime DataAquisicao { get; set; }

    [StringLength(255)]
    public string? LocalizacaoAtual { get; set; }
}