using Dislana.Application.Common.Interfaces;

namespace Dislana.Infrastructure.Services.Cache
{
    /// <summary>
    /// Implementación de cache que no almacena nada. 
    /// Útil para desarrollo cuando Redis no está disponible.
    /// </summary>
    public class NullCacheService : ICacheService
    {
        public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
        {
            // Siempre retorna null, forzando la consulta a la base de datos
            return Task.FromResult<T?>(null);
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
        {
            // No hace nada, ignora el almacenamiento
            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            // No hace nada
            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
        {
            // Siempre retorna false
            return Task.FromResult(false);
        }
    }
}
