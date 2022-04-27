using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Notes.Blazor.Client;
using Notes.Blazor.Client.Services;
using Notes.Blazor.Client.ViewModels.UploadFiles;
using System.Text;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient<IUploadFileService, UploadFileService>(client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
builder.Services.AddHttpClient("webapi", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
builder.Services.AddScoped<IndexViewModel>();
builder.Services.AddScoped<CreateViewModel>();

await builder.Build().RunAsync();
