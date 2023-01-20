using Havit.Blazor.Components.Web;
using Microsoft.AspNetCore.StaticFiles;
using Havit.Blazor.Components.Web.Bootstrap;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.FileProviders;
using number_game;
using number_game.Helpers;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddHxServices();
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddSingleton<GameDataHelper>();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

var provider = new FileExtensionContentTypeProvider
{
    Mappings =
    {
        [".wav"] = "Sound"
    }
};

builder.Services.Configure<StaticFileOptions>(options =>
{
    options.ContentTypeProvider = provider;
});


var app = builder.Build();
await app.RunAsync();
