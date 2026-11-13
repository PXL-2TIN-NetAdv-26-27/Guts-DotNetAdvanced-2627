using ToDoListApp.Domain;

namespace ToDoListApp.AppLogic;

public interface IToDoListRepository
{
    IList<ToDoList> Find(string? titleFilter);
    ToDoList? GetById(Guid id);
    ToDoList AddNew(string title);
    void AddItemToExistingList(Guid listId, string itemDescription);
}