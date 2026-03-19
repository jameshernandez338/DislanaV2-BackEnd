namespace Dislana.Application.Secrets
{
    /// <summary>
    /// Abstraction for retrieving secrets/configuration values.
    /// Implementations can read from IConfiguration, environment variables, or secret stores (Key Vault).
    /// </summary>
    public interface ISecretProvider
    {
        /// <summary>
        /// Get a secret value by key. Returns null when not found.
        /// </summary>
        string? GetSecret(string key);
    }
}
