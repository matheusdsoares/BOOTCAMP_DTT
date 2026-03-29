using System;
using Xunit;
using MinhaApi.Models;

namespace MinhaApi.Tests.Models
{
    public class LoteMinerioTests
    {
        [Fact]
        public void Deve_Criar_LoteMinerio_Com_Valores_Padrao()
        {
            // Act
            var lote = new LoteMinerio();

            // Assert
            Assert.Equal(0, lote.Id);
            Assert.Equal(string.Empty, lote.CodigoLote);
            Assert.Equal(string.Empty, lote.MinaOrigem);
            Assert.Equal(string.Empty, lote.LocalizacaoAtual);

            Assert.Equal(0m, lote.TeorFe);
            Assert.Equal(0m, lote.Umidade);
            Assert.Null(lote.SiO2);
            Assert.Null(lote.P);
            Assert.Equal(0m, lote.Toneladas);

            Assert.Equal(StatusLote.EmEstoque, lote.Status); // enum default = 0
            Assert.Equal(default(DateTime), lote.DataProducao);
        }

        [Fact]
        public void Deve_Permitir_Atribuir_Dados_Ao_LoteMinerio()
        {
            // Arrange
            var dataProducao = new DateTime(2026, 1, 15);

            // Act
            var lote = new LoteMinerio
            {
                Id = 10,
                CodigoLote = "MNA-2026-000123",
                MinaOrigem = "Carajás N4E",
                TeorFe = 65.8m,
                Umidade = 8.5m,
                SiO2 = 2.3m,
                P = 0.04m,
                Toneladas = 12000m,
                DataProducao = dataProducao,
                Status = StatusLote.EmTransporte,
                LocalizacaoAtual = "EFVM - Trem 123"
            };

            // Assert
            Assert.Equal(10, lote.Id);
            Assert.Equal("MNA-2026-000123", lote.CodigoLote);
            Assert.Equal("Carajás N4E", lote.MinaOrigem);
            Assert.Equal(65.8m, lote.TeorFe);
            Assert.Equal(8.5m, lote.Umidade);
            Assert.Equal(2.3m, lote.SiO2);
            Assert.Equal(0.04m, lote.P);
            Assert.Equal(12000m, lote.Toneladas);
            Assert.Equal(dataProducao, lote.DataProducao);
            Assert.Equal(StatusLote.EmTransporte, lote.Status);
            Assert.Equal("EFVM - Trem 123", lote.LocalizacaoAtual);
        }

        [Theory]
        [InlineData(StatusLote.EmEstoque)]
        [InlineData(StatusLote.EmTransporte)]
        [InlineData(StatusLote.Embarcado)]
        public void Deve_Aceitar_Todos_Os_Status_Do_Enum(StatusLote status)
        {
            // Act
            var lote = new LoteMinerio
            {
                Status = status
            };

            // Assert
            Assert.Equal(status, lote.Status);
        }
    }
}

