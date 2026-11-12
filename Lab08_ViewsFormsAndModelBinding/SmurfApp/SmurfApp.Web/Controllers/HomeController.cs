using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SmurfApp.AppLogic;
using SmurfApp.Domain;
using SmurfApp.Web.Models;

namespace SmurfApp.Web.Controllers;

public class HomeController : Controller
{
    public HomeController(ILogger<HomeController> logger, ISmurfStore smurfStore)
    {
    }

    public IActionResult Index()
    {
        throw new NotImplementedException();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}