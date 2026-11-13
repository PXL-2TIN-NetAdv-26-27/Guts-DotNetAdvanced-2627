using System.ComponentModel.DataAnnotations;
using ToDoListApp.Domain;

namespace ToDoListApp.Web.Models;

public class ToDoListDetailViewModel
{
    public ToDoList? List { get; }

    public Guid ListId { get; set; }

    public string NewItemDescription { get; set; }

    public ToDoListDetailViewModel()
    {
        NewItemDescription = string.Empty;
    }

    public ToDoListDetailViewModel(ToDoList? list)
    {
        List = list;
        ListId = list?.Id ?? Guid.Empty;
        NewItemDescription = string.Empty;
    }
}