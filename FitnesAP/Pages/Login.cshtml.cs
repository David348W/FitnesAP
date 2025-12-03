using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FitnesAP.Data;

public class LoginModel : PageModel
{
    private readonly UserService _service;
    public LoginModel(UserService service) => _service = service;

    [BindProperty] public string Username { get; set; }
    [BindProperty] public string Password { get; set; }

    public string Message { get; set; }

    public IActionResult OnPost()
    {
        var user = _service.GetUsers()
            .FirstOrDefault(u => u.Username == Username && u.Password == Password);

        if (user == null)
        {
            Message = "Invalid login.";
            return Page();
        }

        // Save to session
        HttpContext.Session.SetString("username", user.Username);
        HttpContext.Session.SetString("role", user.Role);

        return RedirectToPage("/Index");
    }
}

