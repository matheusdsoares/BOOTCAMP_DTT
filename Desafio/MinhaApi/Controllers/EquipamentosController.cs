using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SuaApi.Data;
using SuaApi.Models;
using SuaApi.Dtos;

namespace SuaApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EquipamentosController : ControllerBase
{
    private readonly AppDbContext _context;

    public EquipamentosController(AppDbContext context) => _context = context;

    // POST: api/equipamentos
    [HttpPost]
    public async Task<IActionResult> Create(CreateEquipamentoDto dto)
    {
        var equipamento = new Equipamento 
        { 
            Codigo = dto.Codigo.Trim(),
            Tipo = dto.Tipo,
            Modelo = dto.Modelo,
            Horimetro = dto.Horimetro,
            StatusOperacional = dto.StatusOperacional,
            DataAquisicao = dto.DataAquisicao,
            LocalizacaoAtual = dto.LocalizacaoAtual
        };

        _context.Equipamentos.Add(equipamento);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = equipamento.Id }, equipamento);
    }

    // GET: api/equipamentos
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _context.Equipamentos.ToListAsync());

    // GET: api/equipamentos/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var equipamento = await _context.Equipamentos.FindAsync(id);
        
        if (equipamento == null) 
            return NotFound(new { message = "Equipamento não encontrado." });
        
        return Ok(equipamento);
    }

    // PUT: api/equipamentos/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CreateEquipamentoDto dto)
    {
        var equipamento = await _context.Equipamentos.FindAsync(id);
        
        if (equipamento == null) 
            return NotFound(new { message = "Equipamento não encontrado para atualização." });

        // Regra de Negócio: Horímetro não pode retroceder
        if (dto.Horimetro < equipamento.Horimetro)
        {
            return BadRequest(new { message = $"O novo horímetro ({dto.Horimetro}) não pode ser menor que o atual ({equipamento.Horimetro})." });
        }

        // Atualização dos campos
        equipamento.Codigo = dto.Codigo.Trim();
        equipamento.Tipo = dto.Tipo;
        equipamento.Modelo = dto.Modelo;
        equipamento.Horimetro = dto.Horimetro;
        equipamento.StatusOperacional = dto.StatusOperacional;
        equipamento.DataAquisicao = dto.DataAquisicao;
        equipamento.LocalizacaoAtual = dto.LocalizacaoAtual;

        await _context.SaveChangesAsync();
        
        return Ok(new { message = "Equipamento atualizado com sucesso!", data = equipamento });
    }

    // DELETE: api/equipamentos/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var equipamento = await _context.Equipamentos.FindAsync(id);
        
        if (equipamento == null) 
            return NotFound(new { message = "Equipamento não encontrado para exclusão." });

        _context.Equipamentos.Remove(equipamento);
        await _context.SaveChangesAsync();

        return Ok(new { message = $"Equipamento '{equipamento.Codigo}' removido com sucesso." });
    }
}