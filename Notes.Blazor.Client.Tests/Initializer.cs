using System.Runtime.CompilerServices;
using System.Text;

namespace Notes.Blazor.Client.Tests;

internal static class Initializer
{
    [ModuleInitializer]
    internal static void Initialize()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }
}
