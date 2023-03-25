using Discord;

namespace Papmaskinen.Bot.Setup;

internal static class Emotes
{
	internal static readonly IEmote ThumbsUp = Emoji.Parse(":thumbsup:");
	internal static readonly IEmote ThumbsDown = Emoji.Parse(":thumbsdown:");
	internal static readonly IEmote FingersCrossed = Emoji.Parse(":fingers_crossed:");
	internal static readonly IEmote House = Emoji.Parse(":house:");
	internal static readonly IEmote GameDie = Emoji.Parse(":game_die:");

	internal static readonly IEmote Clock1 = Emoji.Parse(":clock1:");
	internal static readonly IEmote Clock2 = Emoji.Parse(":clock2:");
	internal static readonly IEmote Clock3 = Emoji.Parse(":clock3:");
	internal static readonly IEmote Clock4 = Emoji.Parse(":clock4:");
	internal static readonly IEmote Clock5 = Emoji.Parse(":clock5:");
	internal static readonly IEmote Clock6 = Emoji.Parse(":clock6:");
	internal static readonly IEmote Clock7 = Emoji.Parse(":clock7:");
	internal static readonly IEmote Clock8 = Emoji.Parse(":clock8:");
	internal static readonly IEmote Clock9 = Emoji.Parse(":clock9:");
	internal static readonly IEmote Clock10 = Emoji.Parse(":clock10:");
	internal static readonly IEmote Clock11 = Emoji.Parse(":clock11:");
	internal static readonly IEmote Clock12 = Emoji.Parse(":clock12:");
	internal static readonly IEnumerable<IEmote> Clocks =
	new IEmote[12]
	{
		Clock1, Clock2, Clock3, Clock4, Clock5, Clock6,
		Clock7, Clock8, Clock9, Clock10, Clock11, Clock12,
	};
}
