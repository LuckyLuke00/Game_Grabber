using Newtonsoft.Json;
using System.Linq;
using System.Text.RegularExpressions;

namespace GameGrabber.Model
{
    internal class Game
    {
        // Set the maximum number of characters for the title
        private readonly int maxTitleLength = 35;

        // Set the maximum number of characters for the platforms
        private readonly int maxPlatformsLength = 45;

        [JsonProperty(PropertyName = "image")]
        public string ImageUrl { get; set; }

        [JsonProperty(PropertyName = "platforms")]
        public string PlatformsRaw { get; set; }

        [JsonProperty(PropertyName = "open_giveaway")]
        public string GiveawayUrl { get; set; }

        public string Platforms
        {
            get
            {
                return TruncateStringWithEllipsis(PlatformsRaw, maxPlatformsLength);
            }
        }

        [JsonProperty(PropertyName = "title")]
        public string TitleRaw { get; set; }

        [JsonProperty(PropertyName = "worth")]
        public string OriginalPrice { get; set; }

        private string _instructions;

        [JsonProperty(PropertyName = "instructions")]
        public string Instructions
        {
            get
            {
                return _instructions;
            }
            set
            {
                // Convert HTML to plain text before setting the value
                _instructions = ConvertHtmlToPlainText(value);
                // Add an additional newline character after every '\n'
                _instructions = _instructions.Replace("\n", "\n\n");
            }
        }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        public float PriceValue
        {
            get
            {
                // Extract the numeric value from OriginalPrice and parse it as a float
                float price;
                float.TryParse(OriginalPrice?.Replace("$", ""), out price);
                return price;
            }
        }

        public string Title
        {
            get
            {
                // Most titles are in the format "Title (Store) giveaway"
                // We only want the title, so we remove the rest
                string title = TitleRaw?.Split('(')?.First()?.Trim() ?? TitleRaw;

                return TruncateStringWithEllipsis(title, maxTitleLength);
            }
        }

        /// <summary>
        /// Truncates a string with an ellipsis at the end, with an optional space before the ellipsis.
        /// </summary>
        /// <param name="text">The input string to truncate.</param>
        /// <param name="maxLength">The maximum length of the truncated string.</param>
        /// <returns>The truncated string with an ellipsis at the end.</returns>
        private string TruncateStringWithEllipsis(string text, int maxLength)
        {
            if (text == null)
            {
                return null;
            }

            if (text.Length > maxLength)
            {
                string dots = " ...";
                if (text[maxLength - 1] == ' ')
                {
                    dots = "...";
                }
                text = text.Substring(0, maxLength) + dots;
            }

            return text;
        }

        private static string ConvertHtmlToPlainText(string html)
        {
            // Remove HTML tags using regular expressions
            string plainText = Regex.Replace(html, "<.*?>", string.Empty);

            // Replace special characters
            plainText = Regex.Replace(plainText, "&nbsp;", " ");
            plainText = Regex.Replace(plainText, "&lt;", "<");
            plainText = Regex.Replace(plainText, "&gt;", ">");
            plainText = Regex.Replace(plainText, "&amp;", "&");
            plainText = Regex.Replace(plainText, "&quot;", "\"");
            plainText = Regex.Replace(plainText, "&apos;", "'");

            return plainText;
        }
    }
}
