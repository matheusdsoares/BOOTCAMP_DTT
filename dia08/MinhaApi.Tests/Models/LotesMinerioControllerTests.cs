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

        [Fact]
        public async Task FluxoCompleto_LoteMinerio_DeveCobrirTodosOsCenarios()
        {
            // --- ARRANGE ---
            using var db = GetDbContext();
            var controller = new LotesMinerioController(db);

            // 1. DTO Válido para criação
            var dtoValido = new CreateLoteMinerioDto 
            { 
                CodigoLote = "LOTE-001", 
                MinaOrigem = "Mina Norte", 
                LocalizacaoAtual = "Patio 1",
                TeorFe = 65,
                Umidade = 8,
                Toneladas = 1000,
                Status = 1
            };

            // 2. DTOs Inválidos para testar os BadRequests (Validation coverage)
            var dtoSemCodigo = new CreateLoteMinerioDto { CodigoLote = "" };
            var dtoTeorInvalido = new CreateLoteMinerioDto { CodigoLote = "L-02", MinaOrigem = "M1", LocalizacaoAtual = "P1", TeorFe = 150 }; // > 100

            // --- ACT & ASSERT (Testando cada ramificação do Controller) ---

            // A. Testando BadRequests do Create
            var resultBad1 = await controller.Create(dtoSemCodigo);
            Assert.IsType<BadRequestObjectResult>(resultBad1);

            var resultBad2 = await controller.Create(dtoTeorInvalido);
            Assert.IsType<BadRequestObjectResult>(resultBad2);

            // B. Testando Caminho Feliz (Create)
            var resultCreate = await controller.Create(dtoValido);
            var createdResult = Assert.IsType<CreatedAtActionResult>(resultCreate);
            var loteCriado = Assert.IsType<LoteMinerio>(createdResult.Value);
            int idGerado = loteCriado.Id;

            // C. Testando Conflito (Código duplicado)
            var resultConflict = await controller.Create(dtoValido);
            Assert.IsType<ConflictObjectResult>(resultConflict);

            // D. Testando GetById (Sucesso)
            var resultGet = await controller.GetById(idGerado);
            Assert.IsType<OkObjectResult>(resultGet);

            // E. Testando GetById (NotFound)
            var resultGet404 = await controller.GetById(9999);
            Assert.IsType<NotFoundResult>(resultGet404);

            // F. Testando Update (Sucesso)
            var dtoUpdate = new UpdateLoteMinerioDto 
            { 
                CodigoLote = "LOTE-001-MOD", 
                MinaOrigem = "Mina Sul" 
            };
            var resultUpdate = await controller.Update(idGerado, dtoUpdate);
            Assert.IsType<NoContentResult>(resultUpdate);

            // G. Testando Update (NotFound)
            var resultUpdate404 = await controller.Update(9999, dtoUpdate);
            Assert.IsType<NotFoundObjectResult>(resultUpdate404);

            // H. Testando Delete (Sucesso)
            var resultDelete = await controller.Delete(idGerado);
            Assert.IsType<NoContentResult>(resultDelete);

            // I. Testando Delete (NotFound)
            var resultDelete404 = await controller.Delete(idGerado); // Já foi deletado
            Assert.IsType<NotFoundObjectResult>(resultDelete404);
        }
    }
}