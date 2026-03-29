using System;
using Xunit;
using MinhaApi.Dtos;

namespace MinhaApi.Tests.Dtos
{
    public class CreateLoteMinerioDtoTests
    {
        [Fact]
        public void Deve_Criar_Dto_Com_Valores_Padrao()
        {
            // Act
            var dto = new CreateLoteMinerioDto();

            // Assert
            Assert.Equal(string.Empty, dto.CodigoLote);
            Assert.Equal(string.Empty, dto.MinaOrigem);
            Assert.Equal(string.Empty, dto.LocalizacaoAtual);

            Assert.Equal(0m, dto.TeorFe);
            Assert.Equal(0m, dto.Umidade);
            Assert.Equal(0m, dto.Toneladas);

            Assert.Null(dto.SiO2);
            Assert.Null(dto.P);
            Assert.Null(dto.DataProducao);

            Assert.Equal(0, dto.Status); // default int = 0
        }

        [Fact]
        public void Deve_Permitir_Preencher_Todos_Os_Campos()
        {
            // Arrange
            var dataProducao = new DateTime(2026, 2, 1);

            // Act
            var dto = new CreateLoteMinerioDto
            {
                CodigoLote = "MNA-2026-000456",
                MinaOrigem = "Carajás N5W",
                TeorFe = 66.2m,
                Umidade = 7.9m,
                SiO2 = 1.8m,
                P = 0.03m,
                Toneladas = 15000m,
                DataProducao = dataProducao,
                Status = 1,
                LocalizacaoAtual = "EFVM - Trem 456"
            };

            // Assert
            Assert.Equal("MNA-2026-000456", dto.CodigoLote);
            Assert.Equal("Carajás N5W", dto.MinaOrigem);
            Assert.Equal(66.2m, dto.TeorFe);
            Assert.Equal(7.9m, dto.Umidade);
            Assert.Equal(1.8m, dto.SiO2);
            Assert.Equal(0.03m, dto.P);
            Assert.Equal(15000m, dto.Toneladas);
            Assert.Equal(dataProducao, dto.DataProducao);
            Assert.Equal(1, dto.Status);
            Assert.Equal("EFVM - Trem 456", dto.LocalizacaoAtual);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void Deve_Aceitar_Status_Validos(int status)
        {
            // Act
            var dto = new CreateLoteMinerioDto
            {
                Status = status
            };

            // Assert
            Assert.Equal(status, dto.Status);
        }
    }
}
