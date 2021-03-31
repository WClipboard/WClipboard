using System.Text;
using System.Text.Json;

namespace WClipboard.Core.Utilities.Json
{
    public class SnakeCaseJsonNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            var stringBuilder = new StringBuilder(name);
            for (int i = 0; i < stringBuilder.Length; i++)
            {
                if (char.IsUpper(stringBuilder[i]))
                {
                    stringBuilder[i] = char.ToLowerInvariant(stringBuilder[i]);

                    if (i > 1)
                    {
                        stringBuilder.Insert(i, '_');
                        i += 1;
                    }
                }
            }

            return stringBuilder.ToString();
        }
    }
}
