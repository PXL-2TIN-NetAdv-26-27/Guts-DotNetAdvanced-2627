using System.ComponentModel.DataAnnotations;

namespace ToDoListApp.Web.Models;

public class AddNewToDoListModel
{
    [Required, MinLength(4)]
    public string Title { get; set; } = null!;
}