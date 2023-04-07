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
Voting Emojis: (\:clockX\: where X is the current month)
January: :clock1:
February: :clock2:
March: :clock3:
April: :clock4:
May: :clock5: 
June: :clock6: 
July: :clock7: 
August: :clock8:
September: :clock9:
October: :clock10:
November: :clock11:
December: :clock12:

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
