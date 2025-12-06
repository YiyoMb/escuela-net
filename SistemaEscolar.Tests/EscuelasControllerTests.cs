using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using SistemaEscolar.API.Models;
using Xunit;

namespace SistemaEscolar.Tests
{
    public class EscuelasControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _jsonOptions;

        public EscuelasControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        [Fact]
        public async Task GetEscuelas_ReturnsListOfSchools()
        {
            // Act
            var response = await _client.GetAsync("/api/escuelas");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var escuelas = await response.Content.ReadFromJsonAsync<List<Escuela>>(_jsonOptions);
            escuelas.Should().NotBeEmpty();
            escuelas.Should().HaveCount(c => c >= 2); // Las 2 del seed
        }

        [Fact]
        public async Task PostEscuela_CreatesNewSchool()
        {
            // Arrange
            var nuevaEscuela = new
            {
                nombre = "Instituto Tecnológico de Querétaro",
                direccion = "Av. Tecnológico s/n",
                telefono = "4421234567"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/escuelas", nuevaEscuela);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            
            var escuelaCreada = await response.Content.ReadFromJsonAsync<Escuela>(_jsonOptions);
            escuelaCreada.Should().NotBeNull();
            escuelaCreada!.Nombre.Should().Be("Instituto Tecnológico de Querétaro");
        }
    }
}