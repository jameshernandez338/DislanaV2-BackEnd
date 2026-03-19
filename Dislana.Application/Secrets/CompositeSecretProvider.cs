namespace Dislana.Application.Secrets
{
    public class CompositeSecretProvider : ISecretProvider
    {
        private readonly IReadOnlyList<ISecretProvider> _providers;
        public CompositeSecretProvider(IEnumerable<ISecretProvider> providers)
        {
            _providers = providers?.ToList() ?? new List<ISecretProvider>();
        }
        public string? GetSecret(string key)
        {
            foreach (var p in _providers)
            {
                try
                {
                    var v = p.GetSecret(key);
                    if (!string.IsNullOrWhiteSpace(v)) return v;
                }
                catch
                {
                    // ignore provider errors and try next
                }
            }
            return null;
        }
    }
}
