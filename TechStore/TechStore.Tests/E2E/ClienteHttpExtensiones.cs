using System.Net;
using System.Text.RegularExpressions;

namespace TechStore.Tests.E2E
{
    public static class ClienteHttpExtensiones
    {
        // Lee el token antiforgery que ASP.NET Core inserta en los formularios.
        public static async Task<string> ObtenerTokenAsync(this HttpClient cliente, string url)
        {
            var html = await cliente.GetStringAsync(url);
            var coincidencia = Regex.Match(html, "name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"");

            if (!coincidencia.Success)
            {
                throw new InvalidOperationException($"No se encontró el token antiforgery en {url}.");
            }

            return coincidencia.Groups[1].Value;
        }

        public static async Task<HttpResponseMessage> IniciarSesionAsync(this HttpClient cliente, string correo, string clave)
        {
            var token = await cliente.ObtenerTokenAsync("/Account/Login");
            var formulario = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["Correo"] = correo,
                ["Clave"] = clave,
                ["__RequestVerificationToken"] = token
            });

            return await cliente.PostAsync("/Account/Login", formulario);
        }

        public static async Task<HttpResponseMessage> EnviarFormularioAsync(
            this HttpClient cliente, string urlFormulario, string urlEnvio, Dictionary<string, string> campos)
        {
            campos["__RequestVerificationToken"] = await cliente.ObtenerTokenAsync(urlFormulario);
            return await cliente.PostAsync(urlEnvio, new FormUrlEncodedContent(campos));
        }

        // Devuelve el HTML con las entidades decodificadas (por ejemplo &#xF3; pasa a ó).
        public static async Task<string> LeerHtmlAsync(this HttpResponseMessage respuesta)
        {
            var html = await respuesta.Content.ReadAsStringAsync();
            return WebUtility.HtmlDecode(html);
        }
    }
}
