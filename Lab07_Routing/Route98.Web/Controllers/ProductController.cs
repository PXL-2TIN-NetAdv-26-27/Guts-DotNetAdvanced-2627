using Microsoft.AspNetCore.Mvc;
using Route98.Web.ViewModels;

namespace Route98.Web.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Detail(Guid id)
        {
            return View(id);
        }

        public IActionResult Overview(string categoryCode, int productsPerPage = 10)
        {
            return View(new ProductOverviewViewModel{ CategoryCode = categoryCode, ProductsPerPage = productsPerPage});
        }
    }
}
