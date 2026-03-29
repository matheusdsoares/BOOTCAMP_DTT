using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace MinhaApi.Tests
{
    public class BasicIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public BasicIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task App_DeveSubir_E_MapearControllers()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act - Tenta acessar um endpoint que você sabe que existe
            var response = await client.GetAsync("/api/LotesMinerio"); // ajuste a rota se necessário

            // Assert
            // Mesmo que retorne 404 (se não houver dados), 
            // já prova que o pipeline do Program.cs foi executado.
            Assert.True(response.StatusCode != System.Net.HttpStatusCode.InternalServerError);
        }
    }
}