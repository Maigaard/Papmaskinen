using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
	.ConfigureFunctionsWorkerDefaults()
	.ConfigureServices(ConfigureServices)
	.Build();
host.Run();

static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
{
	services.AddScoped<Discord.Rest.DiscordRestClient>();
}
