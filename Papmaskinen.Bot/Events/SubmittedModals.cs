using Discord.WebSocket;
using Papmaskinen.Bot.Models;

namespace Papmaskinen.Bot.Events;

public class SubmittedModals
{
    public Task ModalSubmitted(SocketModal modal)
    {
        var bggLinkComponent = modal.Data.Components.Single(x => x.CustomId == "bgg-link");
        var nomination = new Nomination
        {
            BoardGameGeekUrl = bggLinkComponent.Value
        };
        
        return Task.CompletedTask;
    }
}