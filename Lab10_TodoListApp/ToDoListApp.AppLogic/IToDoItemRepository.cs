using ToDoListApp.Domain;

namespace ToDoListApp.AppLogic;

public interface IToDoItemRepository
{
    int GetTotalOfItemsThatAreNotDone();
    ToDoItem Update(Guid id, bool isDone);
}