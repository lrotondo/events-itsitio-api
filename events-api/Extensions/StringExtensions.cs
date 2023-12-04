using System.Globalization;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace events_api.Extensions
{
    public static class StringExtensions
    {
        static readonly Regex WordDelimiters = new Regex(@"[\s—–_]", RegexOptions.Compiled);
        static readonly Regex InvalidChars = new Regex(@"[^a-z0-9\-]", RegexOptions.Compiled);
        static readonly Regex MultipleHyphens = new Regex(@"-{2,}", RegexOptions.Compiled);

        public async static Task<bool> IsImageUrl(this string URL, HttpClient client)
        {
            try
            {
                var response = await client.GetAsync(URL);
                return response.IsSuccessStatusCode && response.Content.Headers.ContentType.MediaType.StartsWith("image");
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string ToUrlSlug(this string value)
        {
            value = value.ToLowerInvariant();
            value = RemoveDiacritics(value);
            value = WordDelimiters.Replace(value, "-");
            value = InvalidChars.Replace(value, "");
            value = MultipleHyphens.Replace(value, "-");
            return value.Trim('-');
        }

        private static string RemoveDiacritics(string stIn)
        {
            string stFormD = stIn.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            for (int ich = 0; ich < stFormD.Length; ich++)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[ich]);
                }
            }

            return (sb.ToString().Normalize(NormalizationForm.FormC));
        }

        /*public static string BuildPostgresConnectionString(this string url)
        {
            var databaseUri = new Uri(url);
            var userInfo = databaseUri.UserInfo.Split(':');

            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/')
            };

            return builder.ToString();
        }*/

    }
}
