using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AhmedOS.Web.Pages;

[Authorize]
public class ReviewsModel : PageModel
{
    public void OnGet() { }
}
