using Microsoft.AspNetCore.Mvc.Rendering;
using SmurfApp.Domain;

namespace SmurfApp.Web.Models;

public class AddOrUpdateSmurfViewModel
{
    public Smurf Smurf { get; set; }

    public IList<SelectListItem> AllCategories { get; }


    public AddOrUpdateSmurfViewModel()
    {
        Smurf = new Smurf();

        // Populate AllCategories with all possible Category enum values
        AllCategories = Enum.GetValues<Category>()
            .Select(c => new SelectListItem
            {
                Value = c.ToString(),
                Text = c.ToString()
            }).ToList();
    }
}