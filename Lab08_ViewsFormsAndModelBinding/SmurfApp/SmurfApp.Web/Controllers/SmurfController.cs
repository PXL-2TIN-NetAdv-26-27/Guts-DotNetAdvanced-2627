using Microsoft.AspNetCore.Mvc;
using SmurfApp.AppLogic;
using SmurfApp.Web.Models;

namespace SmurfApp.Web.Controllers;

public class SmurfController : Controller
{
    public SmurfController(ILogger<SmurfController> logger, ISmurfStore smurfStore)
    {
    }

    public IActionResult AddOrUpdate(Guid? id)
    {
        throw new NotImplementedException();
    }

    public IActionResult AddOrUpdate(Guid? id, AddOrUpdateSmurfViewModel model)
    {
        throw new NotImplementedException();
    }
}