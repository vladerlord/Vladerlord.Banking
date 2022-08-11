using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gateway.Root.Identity.Presentation.Views;

public class UserRegistrationConfirmation : PageModel
{
	public bool IsSuccess { get; init; }
}