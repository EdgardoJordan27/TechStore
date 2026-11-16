using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace TechStore.Infrastructure
{
    // Las tablas de Identity usan índices filtrados en SQL Server. Si la conexión
    // tiene NUMERIC_ROUNDABORT activo, los INSERT/UPDATE sobre esas tablas fallan.
    // Se desactiva cada vez que se abre una conexión.
    public class NumericRoundAbortInterceptor : DbConnectionInterceptor
    {
        public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
        {
            using var comando = connection.CreateCommand();
            comando.CommandText = "SET NUMERIC_ROUNDABORT OFF";
            comando.ExecuteNonQuery();
        }

        public override async Task ConnectionOpenedAsync(DbConnection connection,
            ConnectionEndEventData eventData, CancellationToken cancellationToken = default)
        {
            await using var comando = connection.CreateCommand();
            comando.CommandText = "SET NUMERIC_ROUNDABORT OFF";
            await comando.ExecuteNonQueryAsync(cancellationToken);
        }
    }
}
