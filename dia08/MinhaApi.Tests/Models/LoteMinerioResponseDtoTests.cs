using System;
using Xunit;
using MinhaApi.Dtos;
using MinhaApi.Models;

namespace MinhaApi.Tests.Dtos
{
    public class LoteMinerioResponseDtoTests
    {
        [Fact]
        public void Deve_Criar_LoteMinerioResponseDto_Com_Dados_Corretos()
        {
            // Arrange
            var dataProducao = new DateTime(2026, 3, 10);

            // Act
            var dto = new LoteMinerioResponseDto(
                Id: 1,
                CodigoLote: "MNA-2026-000789",
                MinaOrigem: "Carajás N4E",
                TeorFe: 67.2m,
                Umidade: 7.5m,
                SiO2: 1.9m,
                P: 0.02m,
                Toneladas: 18000m,
                DataProducao: dataProducao,
                Status: StatusLote.Embarcado,
                LocalizacaoAtual: "Porto de Tubarão"
            );

            // Assert
            Assert.Equal(1, dto.Id);
            Assert.Equal("MNA-2026-000789", dto.CodigoLote);
            Assert.Equal("Carajás N4E", dto.MinaOrigem);
            Assert.Equal(67.2m, dto.TeorFe);
            Assert.Equal(7.5m, dto.Umidade);
            Assert.Equal(1.9m, dto.SiO2);
            Assert.Equal(0.02m, dto.P);
            Assert.Equal(18000m, dto.Toneladas);
            Assert.Equal(dataProducao, dto.DataProducao);
            Assert.Equal(StatusLote.Embarcado, dto.Status);
            Assert.Equal("Porto de Tubarão", dto.LocalizacaoAtual);
        }

        [Fact]
        public void Records_Com_Mesmos_Valores_Devem_Ser_Iguais()
        {
            // Arrange
            var data = new DateTime(2026, 3, 10);

            var dto1 = new LoteMinerioResponseDto(
                1, "COD-001", "Mina A", 65m, 8m, null, null,
                10000m, data, StatusLote.EmEstoque, "Pátio"
            );

            var dto2 = new LoteMinerioResponseDto(
                1, "COD-001", "Mina A", 65m, 8m, null, null,
                10000m, data, StatusLote.EmEstoque, "Pátio"
            );

            // Act & Assert
            Assert.Equal(dto1, dto2);
        }

        [Theory]
        [InlineData(StatusLote.EmEstoque)]
        [InlineData(StatusLote.EmTransporte)]
        [InlineData(StatusLote.Embarcado)]
        public void Deve_Aceitar_Todos_Os_Status_Do_Enum(StatusLote status)
        {
            // Act
            var dto = new LoteMinerioResponseDto(
                1, "COD", "Mina", 60m, 5m, null, null,
                5000m, DateTime.Now, status, "Local"
            );

            // Assert
            Assert.Equal(status, dto.Status);
        }
    }
}
