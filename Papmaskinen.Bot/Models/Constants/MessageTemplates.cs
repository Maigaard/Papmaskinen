namespace Papmaskinen.Bot.Models.Constants;

internal class MessageTemplates
{
	internal const string NextEventMessage = @"
- Next scheduled meet up: {0}
- Place: TBD 
- Time: 17:00
- Game Master: TBD
- Primary game: TBD
- Game suggestions: reply to this message with wishes/suggestions

Attending:
TBD
Hopefully:
TBD

You can use the following reactions to interact with this message.
:thumbsup: \: Attending
:fingers_crossed: \: Hopefully attending
:thumbsdown: \: Not attending
:house: \: I can host
:game_die: \: I can be Game Master
@everyone";

	internal const string NominationPinnedMessage = @"
This place is for nominating and voting on games for future PapClub events.

Procedure: To nominate a game use command 'nominate', and add a boardgamegeek link to your game. 

Every month each member has one vote, to be used on any of the nominations.
A week before an event the highest voted nomination is chosen as the primary game.

Voting Emoji: (\:clockX\: where X is the current month)
January: 🕐
February: 🕑
March: 🕒
April: 🕓
May: 🕔 
June: 🕕 
July: 🕖 
August: 🕗
September: 🕘
October: 🕙
November: 🕚
December: 🕛

Have fun! 😁 🎫

Votes:
";

	internal const string NewNominationTemplate = @"
{Name} ({Rating:N1} BGG rating)
{Link}

{Players} players

Mechanics:
{Mechanics:list: - {}|\n}

{Description}";
}
