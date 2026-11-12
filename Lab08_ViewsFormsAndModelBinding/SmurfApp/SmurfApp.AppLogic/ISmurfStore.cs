using SmurfApp.Domain;

namespace SmurfApp.AppLogic;

public interface ISmurfStore
{
    IList<Smurf> GetAll();
    Smurf? GetById(Guid id);
    void AddOrUpdate(Smurf smurf);
    int GetTotal();
}