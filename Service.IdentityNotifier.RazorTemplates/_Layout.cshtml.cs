using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Service.IdentityNotifier.RazorTemplates;

public class Layout : PageModel
{
    public readonly string Title;

    public Layout(string title)
    {
        Title = title;
    }
}
