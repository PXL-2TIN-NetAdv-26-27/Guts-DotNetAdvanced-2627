using SmurfApp.AppLogic;
using SmurfApp.Domain;

namespace SmurfApp.Infrastructure;

public class SmurfDbStore : ISmurfStore
{
    private readonly SmurfDbContext _context;

    public SmurfDbStore(SmurfDbContext context)
    {
        _context = context;
    }

    public void AddOrUpdate(Smurf smurf)
    {
        Smurf? originalSmurf = GetById(smurf.Id);
        if (originalSmurf is not null)
        {
            // Update the original smurf's properties
            _context.Entry(originalSmurf).CurrentValues.SetValues(smurf);
        }
        else
        {
            _context.Smurfs.Add(smurf);
        }

        _context.SaveChanges();
    }

    public IList<Smurf> GetAll()
    {
        return _context.Smurfs.OrderBy(s => s.Name).ToList();
    }

    public Smurf? GetById(Guid id)
    {
        return _context.Smurfs.Find(id);
    }

    public int GetTotal()
    {
        return _context.Smurfs.Count();
    }
}