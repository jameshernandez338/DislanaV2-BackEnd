namespace Dislana.Application.Secrets
{
    public class ConfigurationSecretProvider : ISecretProvider
    {
        private readonly Func<string, string?> _resolver;

        public ConfigurationSecretProvider(Func<string, string?> resolver)
        {
            _resolver = resolver;
        }

        public string? GetSecret(string key)
        {
            return _resolver(key);
        }
    }
}
