using System;
using Xunit;
using MinhaApi.Dtos;

namespace MinhaApi.Tests.Dtos
{
    public class UpdateLoteMinerioDtoTests
    {
        [Fact]
        public void Deve_Criar_UpdateDto_Com_Valores_Default()
        {
            // Act
            var dto = new UpdateLoteMinerioDto();

            // Assert
            Assert.Null(dto.CodigoLote);
            Assert.Null(dto.MinaOrigem);
            Assert.Null(dto.LocalizacaoAtual);

            Assert.Equal(0m, dto.TeorFe);
            Assert.Equal(0m, dto.Umidade);
            Assert.Null(dto.SiO2);
            Assert.Null(dto.P);
            Assert.Equal(0m, dto.Toneladas);
            Assert.Equal(default(DateTime), dto.DataProducao);
            Assert.Equal(0, dto.Status);
        }

        [Fact]
        public void Deve_Permitir_Atualizar_Todos_Os_Campos()
        {
            // Arrange
            var dataProducao = new DateTime(2026, 4, 20);

            // Act
            var dto = new UpdateLoteMinerioDto
            {
                CodigoLote = "MNA-2026-000999",
                MinaOrigem = "Carajás N5S",
                TeorFe = 66.7m,
                Umidade = 7.8m,
                SiO2 = 2.0m,
                P = 0.05m,
                Toneladas = 14000m,
                DataProducao = dataProducao,
                Status = 2,
                LocalizacaoAtual = "Porto de Tubarão"
            };

            // Assert
            Assert.Equal("MNA-2026-000999", dto.CodigoLote);
            Assert.Equal("Carajás N5S", dto.MinaOrigem);
            Assert.Equal(66.7m, dto.TeorFe);
            Assert.Equal(7.8m, dto.Umidade);
            Assert.Equal(2.0m, dto.SiO2);
            Assert.Equal(0.05m, dto.P);
            Assert.Equal(14000m, dto.Toneladas);
            Assert.Equal(dataProducao, dto.DataProducao);
            Assert.Equal(2, dto.Status);
            Assert.Equal("Porto de Tubarão", dto.LocalizacaoAtual);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void Deve_Aceitar_Status_Validos(int status)
        {
            // Act
            var dto = new UpdateLoteMinerioDto
            {
                Status = status
            };

            // Assert
            Assert.Equal(status, dto.Status);
        }
    }
}
