using System.Net;
using TechStore.Data;

namespace TechStore.Tests.E2E
{
    // Pruebas de extremo a extremo: recorren los flujos de usuario sobre la aplicación
    // completa (inicio de sesión, roles, productos, restricciones, errores y reportes).
    public class FlujosE2ETests : IClassFixture<TechStoreFactory>
    {
        private readonly TechStoreFactory _fabrica;

        public FlujosE2ETests(TechStoreFactory fabrica)
        {
            _fabrica = fabrica;
        }

        private Task<HttpClient> ComoAdministrador()
            => _fabrica.CrearClienteAutenticadoAsync(IdentidadInicial.CorreoAdministrador, IdentidadInicial.ClaveAdministrador);

        private Task<HttpClient> ComoVendedor()
            => _fabrica.CrearClienteAutenticadoAsync(IdentidadInicial.CorreoVendedor, IdentidadInicial.ClaveVendedor);

        private Task<HttpClient> ComoCliente()
            => _fabrica.CrearClienteAutenticadoAsync(IdentidadInicial.CorreoCliente, IdentidadInicial.ClaveCliente);

        private static Dictionary<string, string> CamposProducto(string nombre, string stock)
        {
            return new Dictionary<string, string>
            {
                ["Nombre"] = nombre,
                ["Descripcion"] = "Producto registrado desde la prueba E2E",
                ["Precio"] = "99.90",
                ["CategoriaId"] = "1",
                ["Stock"] = stock,
                ["Estado"] = "Activo",
                ["Imagen"] = ""
            };
        }

        private static void AssertAccesoDenegado(HttpResponseMessage respuesta)
        {
            Assert.Equal(HttpStatusCode.Redirect, respuesta.StatusCode);
            Assert.Contains("/Account/AccesoDenegado", respuesta.Headers.Location!.ToString());
        }

        // Inicio de sesión

        [Fact]
        public async Task Login_ConCredencialesCorrectas_RedirigeAlInicio()
        {
            var cliente = _fabrica.CrearCliente();

            var respuesta = await cliente.IniciarSesionAsync(
                IdentidadInicial.CorreoAdministrador, IdentidadInicial.ClaveAdministrador);

            Assert.Equal(HttpStatusCode.Redirect, respuesta.StatusCode);
            Assert.Equal("/", respuesta.Headers.Location!.OriginalString);
        }

        [Fact]
        public async Task Login_ConClaveIncorrecta_MuestraMensajeDeError()
        {
            var cliente = _fabrica.CrearCliente();

            var respuesta = await cliente.IniciarSesionAsync(IdentidadInicial.CorreoCliente, "ClaveEquivocada1");
            var html = await respuesta.LeerHtmlAsync();

            Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
            Assert.Contains("Correo o contraseña incorrectos.", html);
        }

        // Protección de rutas

        [Fact]
        public async Task UsuarioAnonimo_AlEntrarAProductos_EsRedirigidoAlLogin()
        {
            var cliente = _fabrica.CrearCliente();

            var respuesta = await cliente.GetAsync("/Productos");

            Assert.Equal(HttpStatusCode.Redirect, respuesta.StatusCode);
            Assert.Contains("/Account/Login", respuesta.Headers.Location!.ToString());
        }

        // Rol Cliente

        [Fact]
        public async Task Cliente_PuedeConsultarProductos_PeroNoVeBotonesDeAdministracion()
        {
            var cliente = await ComoCliente();

            var respuesta = await cliente.GetAsync("/Productos");
            var html = await respuesta.LeerHtmlAsync();

            Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
            Assert.Contains("Laptop HyperX Blaze Pro", html);
            Assert.DoesNotContain("Nuevo producto", html);
            Assert.DoesNotContain("Reporte PDF", html);
        }

        [Fact]
        public async Task Cliente_AlEntrarAAgregarProducto_RecibeAccesoDenegado()
        {
            var cliente = await ComoCliente();

            var respuesta = await cliente.GetAsync("/Productos/Agregar");

            AssertAccesoDenegado(respuesta);

            var pagina = await cliente.GetAsync(respuesta.Headers.Location!.ToString());
            var html = await pagina.LeerHtmlAsync();

            Assert.Equal(HttpStatusCode.Forbidden, pagina.StatusCode);
            Assert.Contains("Acceso denegado", html);
        }

        [Fact]
        public async Task Cliente_NoPuedeEntrarAReportesNiUsuarios()
        {
            var cliente = await ComoCliente();

            AssertAccesoDenegado(await cliente.GetAsync("/Reportes"));
            AssertAccesoDenegado(await cliente.GetAsync("/Usuarios"));
        }

        // Rol Vendedor

        [Fact]
        public async Task Vendedor_PuedeRegistrarUnProducto()
        {
            var cliente = await ComoVendedor();

            var respuesta = await cliente.EnviarFormularioAsync(
                "/Productos/Agregar", "/Productos/Agregar", CamposProducto("Producto E2E Vendedor", "7"));

            Assert.Equal(HttpStatusCode.Redirect, respuesta.StatusCode);

            var listado = await (await cliente.GetAsync("/Productos")).LeerHtmlAsync();
            Assert.Contains("Producto E2E Vendedor", listado);
        }

        [Fact]
        public async Task Vendedor_PuedeEditarUnProducto()
        {
            var cliente = await ComoVendedor();
            var campos = CamposProducto("Producto Editado E2E", "12");
            campos["Id"] = "3";

            var respuesta = await cliente.EnviarFormularioAsync("/Productos/Editar/3", "/Productos/Editar/3", campos);

            Assert.Equal(HttpStatusCode.Redirect, respuesta.StatusCode);

            var listado = await (await cliente.GetAsync("/Productos")).LeerHtmlAsync();
            Assert.Contains("Producto Editado E2E", listado);
        }

        [Fact]
        public async Task Vendedor_NoPuedeEliminarNiAdministrarUsuarios()
        {
            var cliente = await ComoVendedor();

            AssertAccesoDenegado(await cliente.GetAsync("/Productos/Eliminar/1"));
            AssertAccesoDenegado(await cliente.GetAsync("/Usuarios"));
        }

        [Fact]
        public async Task Vendedor_PuedeEntrarAReportes()
        {
            var cliente = await ComoVendedor();

            var respuesta = await cliente.GetAsync("/Reportes");

            Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        }

        // Rol Administrador

        [Fact]
        public async Task Administrador_PuedeEliminarUnProducto()
        {
            var cliente = await ComoAdministrador();

            var respuesta = await cliente.EnviarFormularioAsync(
                "/Productos/Eliminar/8", "/Productos/Eliminar/8", new Dictionary<string, string>());

            Assert.Equal(HttpStatusCode.Redirect, respuesta.StatusCode);

            var consulta = await cliente.GetAsync("/Productos/Eliminar/8");
            Assert.Equal(HttpStatusCode.NotFound, consulta.StatusCode);
        }

        [Fact]
        public async Task Administrador_VeLosUsuariosConSusRoles()
        {
            var cliente = await ComoAdministrador();

            var respuesta = await cliente.GetAsync("/Usuarios");
            var html = await respuesta.LeerHtmlAsync();

            Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
            Assert.Contains(IdentidadInicial.CorreoAdministrador, html);
            Assert.Contains(IdentidadInicial.CorreoVendedor, html);
            Assert.Contains(IdentidadInicial.CorreoCliente, html);
        }

        // Reportes

        [Fact]
        public async Task Administrador_PuedeDescargarElReportePdf()
        {
            var cliente = await ComoAdministrador();

            var respuesta = await cliente.GetAsync("/Reportes/Pdf");
            var bytes = await respuesta.Content.ReadAsByteArrayAsync();

            Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
            Assert.Equal("application/pdf", respuesta.Content.Headers.ContentType!.MediaType);
            Assert.Equal("%PDF", System.Text.Encoding.ASCII.GetString(bytes, 0, 4));
        }

        [Fact]
        public async Task Vendedor_PuedeDescargarElArchivoExcel()
        {
            var cliente = await ComoVendedor();

            var respuesta = await cliente.GetAsync("/Reportes/Excel");
            var bytes = await respuesta.Content.ReadAsByteArrayAsync();

            Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
            Assert.Contains("spreadsheetml", respuesta.Content.Headers.ContentType!.MediaType);
            Assert.Equal("PK", System.Text.Encoding.ASCII.GetString(bytes, 0, 2));
        }

        // Páginas de error

        [Fact]
        public async Task RutaInexistente_MuestraLaPaginaPersonalizadaDeError()
        {
            var cliente = _fabrica.CrearCliente();

            var respuesta = await cliente.GetAsync("/ruta-que-no-existe");
            var html = await respuesta.LeerHtmlAsync();

            Assert.Equal(HttpStatusCode.NotFound, respuesta.StatusCode);
            Assert.Contains("Página no encontrada", html);
        }

        [Fact]
        public async Task ExcepcionNoControlada_MuestraLaPaginaDeErrorSinDetallesTecnicos()
        {
            var cliente = await ComoAdministrador();

            var respuesta = await cliente.GetAsync("/Home/SimularError");
            var html = await respuesta.LeerHtmlAsync();

            Assert.Equal(HttpStatusCode.InternalServerError, respuesta.StatusCode);
            Assert.Contains("Ocurrió un problema", html);
            Assert.DoesNotContain("InvalidOperationException", html);
        }
    }
}
