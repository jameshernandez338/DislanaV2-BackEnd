using Dislana.Application.Payment.DTOs;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace Dislana.Application.Payment.Utils
{
    internal static class PaymentUtils
    {
        public static string CreateReference()
        {
            var random = Random.Shared.Next(10000000, 99999999);
            return $"ORD-{random}-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
        }

        public static string BuildItemsXml(IReadOnlyList<PaymentItemDto> items)
        {
            var root = new XElement("Items",
                items.Select(i =>
                    new XElement("Item",
                        new XElement("Tipo", i.Tipo ?? string.Empty),
                        new XElement("Documento", i.Documento ?? string.Empty),
                        new XElement("Item", i.Item ?? string.Empty),
                        new XElement("Cantidad", i.Cantidad.ToString(CultureInfo.InvariantCulture)),
                        new XElement("Valor", i.Valor.ToString(CultureInfo.InvariantCulture))
                    )));

            return new XDocument(root).ToString(SaveOptions.DisableFormatting);
        }

        public static string CreateSign(string raw)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(raw));

                var sb = new StringBuilder();
                foreach (var b in bytes)
                    sb.Append(b.ToString("x2"));

                return sb.ToString();
            }
        }
    }
}
