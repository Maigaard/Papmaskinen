using System.Text.RegularExpressions;
using Discord;

namespace Papmaskinen.Bot.Extensions
{
	internal static class MessagePropertiesExtension
	{
		internal static void EditContent(this MessageProperties properties, string prefix, string input, string replacement)
		{
			string pattern = $@"({prefix}:(\s|\r\n))([\w, ]*)(\r\n)";
			properties.Content = Regex.Replace(input, pattern, $"$1{replacement}$4");
		}
	}
}
