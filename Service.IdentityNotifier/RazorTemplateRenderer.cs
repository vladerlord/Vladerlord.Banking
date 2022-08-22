using System.Diagnostics.CodeAnalysis;
using Razor.Templating.Core;

namespace Service.IdentityNotifier;

public class RazorTemplateRenderer
{
    public async Task<string> Render<T>([DisallowNull] T model)
    {
        var viewPath = $"~/{model.GetType().Name}.cshtml";

        return await RazorTemplateEngine.RenderAsync(viewPath, model);
    }
}
