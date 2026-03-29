using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinhaApi.Controllers;
using MinhaApi.Data;
using MinhaApi.Dtos;
using MinhaApi.Models;
using Xunit;

namespace MinhaApi.Tests.Controllers
{
    public class LotesMinerioControllerTests
    {
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        // --- TESTES DE CRIAÇÃO (CREATE) ---

        [Theory]
        [InlineData("", "Mina 1", "Patio 1", 60, 10, 500, 1)] // CodigoLote vazio
        [InlineData("L01", "", "Patio 1", 60, 10, 500, 1)]   // MinaOrigem vazia
        [InlineData("L01", "Mina 1", "", 60, 10, 500, 1)]   // Localizacao vazia
        [InlineData("L01", "Mina 1", "P1", -1, 10, 500, 1)]  // TeorFe < 0
        [InlineData("L01", "Mina 1", "P1", 101, 10, 500, 1)] // TeorFe > 100
        [InlineData("L01", "Mina 1", "P1", 60, -1, 500, 1)]  // Umidade < 0
        [InlineData("L01", "Mina 1", "P1", 60, 101, 500, 1)] // Umidade > 100
        [InlineData("L01", "Mina 1", "P1", 60, 10, 0, 1)]    // Toneladas <= 0
        [InlineData("L01", "Mina 1", "P1", 60, 10, 500, 5)]  // Status inválido
        public async Task Create_DeveRetornarBadRequest_QuandoDadosInvalidos(
            string cod, string mina, string loc, double teor, double umid, double ton, int status)
        {
            using var db = GetDbContext();
            var controller = new LotesMinerioController(db);
            var dto = new CreateLoteMinerioDto { 
                CodigoLote = cod, MinaOrigem = mina, LocalizacaoAtual = loc, 
                TeorFe = teor, Umidade = umid, Toneladas = ton, Status = status 
            };

            var result = await controller.Create(dto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Create_DeveRetornarConflict_QuandoCodigoJaExiste()
        {
            using var db = GetDbContext();
            var controller = new LotesMinerioController(db);
            var dto = new CreateLoteMinerioDto { CodigoLote = "DUPLICADO", MinaOrigem = "M1", LocalizacaoAtual = "P1", Toneladas = 10 };
            
            await controller.Create(dto); // Primeiro insert
            var result = await controller.Create(dto); // Segundo insert (conflito)

            Assert.IsType<ConflictObjectResult>(result);
        }

        // --- TESTES DE ATUALIZAÇÃO (UPDATE) ---

        [Theory]
        [InlineData("", "Mina Alterada")] // CodigoLote vazio no Update
        [InlineData("L01", "")]           // MinaOrigem vazia no Update
        public async Task Update_DeveRetornarBadRequest_QuandoCamposObrigatoriosVazios(string cod, string mina)
        {
            using var db = GetDbContext();
            var controller = new LotesMinerioController(db);
            
            // Criar um lote para tentar atualizar
            var lote = new LoteMinerio { CodigoLote = "ORIGINAL", MinaOrigem = "ORIGINAL", LocalizacaoAtual = "P1" };
            db.LotesMinerio.Add(lote);
            await db.SaveChangesAsync();

            var dtoUpdate = new UpdateLoteMinerioDto { CodigoLote = cod, MinaOrigem = mina };
            var result = await controller.Update(lote.Id, dtoUpdate);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        // --- TESTES DE NOT FOUND ---

        [Fact]
        public async Task GetById_Update_Delete_DevemRetornarNotFound_QuandoIdNaoExiste()
        {
            using var db = GetDbContext();
            var controller = new LotesMinerioController(db);
            int idInexistente = 999;

            var resultGet = await controller.GetById(idInexistente);
            var resultUpdate = await controller.Update(idInexistente, new UpdateLoteMinerioDto { CodigoLote = "A", MinaOrigem = "B" });
            var resultDelete = await controller.Delete(idInexistente);

            Assert.IsType<NotFoundResult>(resultGet);
            Assert.IsType<NotFoundObjectResult>(resultUpdate);
            Assert.IsType<NotFoundObjectResult>(resultDelete);
        }

        // --- CAMINHO FELIZ (FLUXO COMPLETO) ---

        [Fact]
        public async Task FluxoCompleto_Sucesso_DeveCobrirRestante()
        {
            using var db = GetDbContext();
            var controller = new LotesMinerioController(db);

            // 1. Create
            var dto = new CreateLoteMinerioDto { CodigoLote = "SUCESSO", MinaOrigem = "M1", LocalizacaoAtual = "P1", Toneladas = 10, DataProducao = DateTime.Now };
            var resCreate = await controller.Create(dto);
            var created = Assert.IsType<CreatedAtActionResult>(resCreate);
            var lote = (LoteMinerio)created.Value;

            // 2. GetById
            var resGet = await controller.GetById(lote.Id);
            Assert.IsType<OkObjectResult>(resGet);

            // 3. Update
            var resUpd = await controller.Update(lote.Id, new UpdateLoteMinerioDto { CodigoLote = "ALTERADO", MinaOrigem = "M1" });
            Assert.IsType<NoContentResult>(resUpd);

            // 4. Delete
            var resDel = await controller.Delete(lote.Id);
            Assert.IsType<NoContentResult>(resDel);
        }
    }
}