using System.Text;
using Microsoft.Extensions.Logging;

namespace TechStore.Infrastructure
{
    // Proveedor de logging sencillo: además de la consola, guarda los eventos
    // en Logs/techstore-AAAAMMDD.log dentro de la carpeta del proyecto.
    public class ProveedorLogArchivo : ILoggerProvider
    {
        private readonly string _carpeta;
        private readonly object _bloqueo = new();

        public ProveedorLogArchivo(string carpeta)
        {
            _carpeta = carpeta;
            Directory.CreateDirectory(_carpeta);
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new LoggerArchivo(categoryName, this);
        }

        public void Dispose()
        {
        }

        private void Escribir(string linea)
        {
            var ruta = Path.Combine(_carpeta, $"techstore-{DateTime.Now:yyyyMMdd}.log");

            try
            {
                lock (_bloqueo)
                {
                    File.AppendAllText(ruta, linea + Environment.NewLine, Encoding.UTF8);
                }
            }
            catch (IOException)
            {
                // Un problema al escribir el log no debe interrumpir la aplicación.
            }
        }

        private class LoggerArchivo : ILogger
        {
            private readonly string _categoria;
            private readonly ProveedorLogArchivo _proveedor;

            public LoggerArchivo(string categoria, ProveedorLogArchivo proveedor)
            {
                _categoria = categoria;
                _proveedor = proveedor;
            }

            public IDisposable? BeginScope<TState>(TState state) where TState : notnull
            {
                return null;
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return logLevel >= LogLevel.Information;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
                Exception? exception, Func<TState, Exception?, string> formatter)
            {
                if (!IsEnabled(logLevel))
                {
                    return;
                }

                var linea = new StringBuilder();
                linea.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                linea.Append(" [").Append(logLevel).Append("] ");
                linea.Append(_categoria).Append(": ");
                linea.Append(formatter(state, exception));

                if (exception is not null)
                {
                    linea.Append(Environment.NewLine).Append(exception);
                }

                _proveedor.Escribir(linea.ToString());
            }
        }
    }
}
