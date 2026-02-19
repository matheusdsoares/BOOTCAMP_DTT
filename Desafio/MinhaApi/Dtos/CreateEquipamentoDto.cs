using System.ComponentModel.DataAnnotations;
using SuaApi.Models;

namespace SuaApi.Dtos;

public record CreateEquipamentoDto(
    [Required(ErrorMessage = "O código é obrigatório.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "O código deve ter entre 3 e 50 caracteres.")]
    string Codigo,

    [Required(ErrorMessage = "O tipo de equipamento é obrigatório.")]
    TipoEquipamento Tipo,

    [Required(ErrorMessage = "O modelo é obrigatório.")]
    [StringLength(100)]
    string Modelo,

    [Range(0, (double)decimal.MaxValue, ErrorMessage = "O horímetro não pode ser negativo.")]
    decimal Horimetro,

    [Required(ErrorMessage = "O status operacional é obrigatório.")]
    StatusOperacional StatusOperacional,

    [Required(ErrorMessage = "A data de aquisição é obrigatória.")]
    DateTime DataAquisicao,

    [StringLength(255)]
    string? LocalizacaoAtual
);