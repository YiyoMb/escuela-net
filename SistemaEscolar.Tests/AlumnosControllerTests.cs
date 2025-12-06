using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using SistemaEscolar.API.Models;
using Xunit;

namespace SistemaEscolar.Tests
{
    public class AlumnosControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _jsonOptions;

        public AlumnosControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            
            // Configurar opciones de JSON para que no sea case-sensitive
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        // ============================================
        // TEST 1: Obtener lista de alumnos
        // ============================================
        [Fact]
        public async Task GetAlumnos_ReturnsSuccessStatusCode()
        {
            // Act (Acción): Hacer la petición GET
            var response = await _client.GetAsync("/api/alumnos");

            // Assert (Verificación): Debe ser exitosa
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            // Verificar que devuelve JSON
            response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
        }

        // ============================================
        // TEST 2: Obtener listado completo con relaciones
        // ============================================
        [Fact]
        public async Task GetAlumnosCompleto_ReturnsAlumnosWithRelations()
        {
            // Act
            var response = await _client.GetAsync("/api/alumnos/completo");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Leer el contenido JSON
            var content = await response.Content.ReadAsStringAsync();
            var alumnos = JsonSerializer.Deserialize<List<JsonElement>>(content, _jsonOptions);

            // Verificar que hay alumnos (los del Seed Data)
            alumnos.Should().NotBeEmpty();
            alumnos.Should().HaveCount(c => c >= 2); // Al menos los 2 del seed

            // Verificar que el primer alumno tiene las propiedades esperadas
            var primerAlumno = alumnos![0];
            primerAlumno.GetProperty("id").GetInt32().Should().BeGreaterThan(0);
            primerAlumno.GetProperty("nombreCompleto").GetString().Should().NotBeNullOrEmpty();
            
            // Verificar que incluye las relaciones
            primerAlumno.TryGetProperty("padre", out _).Should().BeTrue();
            primerAlumno.TryGetProperty("madre", out _).Should().BeTrue();
            primerAlumno.TryGetProperty("escuela", out _).Should().BeTrue();
        }

        // ============================================
        // TEST 3: Crear un alumno nuevo
        // ============================================
        [Fact]
        public async Task PostAlumno_CreatesNewAlumno()
        {
            // Arrange (Preparación): Crear el objeto a enviar
            var nuevoAlumno = new
            {
                nombre = "Diego",
                apellido = "Morales",
                fechaNacimiento = "2014-03-15T00:00:00.000Z",
                grado = "4° Primaria",
                padreId = 1,  // Juan García (del seed data)
                madreId = 2,  // María Rodríguez (del seed data)
                escuelaId = 1 // Escuela Benito Juárez (del seed data)
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/alumnos", nuevoAlumno);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            // Verificar que devuelve el alumno creado
            var alumnoCreado = await response.Content.ReadFromJsonAsync<Alumno>(_jsonOptions);
            alumnoCreado.Should().NotBeNull();
            alumnoCreado!.Id.Should().BeGreaterThan(0);
            alumnoCreado.Nombre.Should().Be("Diego");
            alumnoCreado.Apellido.Should().Be("Morales");
        }

        // ============================================
        // TEST 4: Obtener alumno inexistente (404)
        // ============================================
        [Fact]
        public async Task GetAlumno_WithInvalidId_ReturnsNotFound()
        {
            // Act: Buscar un alumno con ID que no existe
            var response = await _client.GetAsync("/api/alumnos/99999");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        // ============================================
        // TEST 5: Validar que no se puede crear alumno sin datos requeridos
        // ============================================
        [Fact]
        public async Task PostAlumno_WithoutRequiredFields_ReturnsBadRequest()
        {
            // Arrange: Alumno inválido (sin nombre)
            var alumnoInvalido = new
            {
                apellido = "Test",
                fechaNacimiento = "2014-03-15T00:00:00.000Z",
                padreId = 1,
                madreId = 2,
                escuelaId = 1
                // Falta el campo "nombre" que es requerido
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/alumnos", alumnoInvalido);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}