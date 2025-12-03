using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FitnesAP.Data;

public class RegisterModel : PageModel
{
    private readonly UserService _service = new UserService();

    [BindProperty] public string Username { get; set; }
    [BindProperty] public string Password { get; set; }
    public string Message { get; set; }

    public void OnPost()
    {
        if (_service.Register(Username, Password))
        {
            Message = "Registration successful!";
        }
        else
        {
            Message = "Username already exists!";
        }
    }
}

