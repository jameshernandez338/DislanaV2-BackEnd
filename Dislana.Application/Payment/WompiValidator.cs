using System.Reflection;
using System.Text;
using Dislana.Application.Payment.DTOs;
using Dislana.Application.Payment.Utils;

namespace Dislana.Application.Payment
{
    public static class WompiSignatureValidator
    {
        public static bool Validate(WompiWebhookRequest evt, string secret)
        {
            if (evt == null) return false;

            string concatenated = BuildConcatenation(evt)
                                  + evt.timestamp
                                  + (secret ?? string.Empty);

            string calculated = PaymentUtils.CreateSign(concatenated);

            return string.Equals(calculated, evt.signature?.checksum, StringComparison.OrdinalIgnoreCase);
        }

        private static string BuildConcatenation(WompiWebhookRequest evt)
        {
            var sb = new StringBuilder();

            if (evt.signature?.properties != null)
            {
                foreach (var path in evt.signature.properties)
                {
                    var value = ResolvePath(evt.data, path);
                    sb.Append(value);
                }
            }

            return sb.ToString();
        }

        private static object ResolvePath(object obj, string path)
        {
            var parts = path.Split('.');

            object current = obj ?? new object();

            foreach (var part in parts)
            {
                if (current == null) return string.Empty;

                var prop = current.GetType().GetProperty(part, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (prop == null) return string.Empty;

                current = prop.GetValue(current);
            }

            return current ?? string.Empty;
        }
    }
}
