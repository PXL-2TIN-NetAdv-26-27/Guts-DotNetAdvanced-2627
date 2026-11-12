using System.ComponentModel.DataAnnotations;

namespace SmurfApp.Domain;

public class Smurf
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public int Age { get; set; }

    public string ImageUrl  { get; set; }

    public Category Category { get; set; }

    public Smurf()
    {
        Id = Guid.NewGuid();
        Name = string.Empty;
        Age = 100;
        ImageUrl = string.Empty;
        Category = Category.Skilled;
    }
}