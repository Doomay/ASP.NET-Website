using Microsoft.AspNetCore.Mvc;

public class ContactController : Controller
{
    public IActionResult Contact()
    {
        return View();
    }
}
