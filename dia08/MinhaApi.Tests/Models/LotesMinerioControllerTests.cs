using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;
using MinhaApi.Controllers;
using MinhaApi.Data;
using MinhaApi.Models;
using MinhaApi.Dtos;

namespace MinhaApi.Tests.Controllers
{
    public class LotesMinerioControllerTests
    {
        private AppDbContext GetInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task Create_Deve_Retornar_Created_Quando_Dados_Validos()
        {
            // Arrange
            using var db = GetInMemoryDb();
            var controller = new LotesMinerioController(db);

            var dto = new CreateLoteMinerioDto
            {
                CodigoLote = "MNA-2026-1001",
                MinaOrigem = "Carajás N4E",
                TeorFe = 65.5m,
                Umidade = 8.2m,
                Toneladas = 12000m,
                LocalizacaoAtual = "Pátio Carajás",
                Status = 0
            };

            // Act
            var result = await controller.Create(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var lote = Assert.IsType<LoteMinerio>(createdResult.Value);
            Assert.Equal("MNA-2026-1001", lote.CodigoLote);
        }

        [Fact]
        public async Task Create_Deve_Retornar_BadRequest_Se_CodigoLote_Vazio()
        {
            using var db = GetInMemoryDb();
            var controller = new LotesMinerioController(db);

            var dto = new CreateLoteMinerioDto
            {
                CodigoLote = "",
                MinaOrigem = "Mina A",
                TeorFe = 50,
                Umidade = 5,
                Toneladas = 1000,
                LocalizacaoAtual = "Local",
                Status = 0
            };

            var result = await controller.Create(dto);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("CodigoLote é obrigatório.", badRequest.Value);
        }

        [Fact]
        public async Task GetById_Deve_Retornar_NotFound_Se_Id_Invalido()
        {
            using var db = GetInMemoryDb();
            var controller = new LotesMinerioController(db);

            var result = await controller.GetById(999); // Id que não existe
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Update_Deve_Atualizar_Lote_Quando_Existir()
        {
            using var db = GetInMemoryDb();

            // Arrange: cria lote inicial
            var lote = new LoteMinerio
            {
                CodigoLote = "OLD-001",
                MinaOrigem = "Mina Velha",
                TeorFe = 60,
                Umidade = 5,
                Toneladas = 1000,
                LocalizacaoAtual = "Pátio A",
                Status = StatusLote.EmEstoque,
                DataProducao = DateTime.UtcNow
            };
            db.LotesMinerio.Add(lote);
            await db.SaveChangesAsync();

            var controller = new LotesMinerioController(db);

            // Act: atualiza lote
            var updateDto = new UpdateLoteMinerioDto
            {
                CodigoLote = "NEW-001",
                MinaOrigem = "Mina Nova",
                TeorFe = 62,
                Umidade = 6,
                Toneladas = 1200,
                LocalizacaoAtual = "Pátio B",
                Status = 1,
                DataProducao = lote.DataProducao
            };

            var result = await controller.Update(lote.Id, updateDto);

            // Assert
            Assert.IsType<NoContentResult>(result);

            var updatedLote = await db.LotesMinerio.FindAsync(lote.Id);
            Assert.Equal("NEW-001", updatedLote!.CodigoLote);
            Assert.Equal("Mina Nova", updatedLote.MinaOrigem);
            Assert.Equal(62m, updatedLote.TeorFe);
        }

        [Fact]
        public async Task Delete_Deve_Remover_Lote_Quando_Existir()
        {
            using var db = GetInMemoryDb();

            var lote = new LoteMinerio
            {
                CodigoLote = "DEL-001",
                MinaOrigem = "Mina A",
                TeorFe = 60,
                Umidade = 5,
                Toneladas = 1000,
                LocalizacaoAtual = "Pátio",
                Status = StatusLote.EmEstoque,
                DataProducao = DateTime.UtcNow
            };
            db.LotesMinerio.Add(lote);
            await db.SaveChangesAsync();

            var controller = new LotesMinerioController(db);

            var result = await controller.Delete(lote.Id);

            Assert.IsType<NoContentResult>(result);
            var deletedLote = await db.LotesMinerio.FindAsync(lote.Id);
            Assert.Null(deletedLote);
        }
    }
}
