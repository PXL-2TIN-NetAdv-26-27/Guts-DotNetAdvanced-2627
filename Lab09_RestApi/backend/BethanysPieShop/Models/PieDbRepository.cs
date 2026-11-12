using Microsoft.EntityFrameworkCore;

namespace BethanysPieShop.Models
{
    public class PieDbRepository : IPieRepository
    {
        private readonly BethanysPieShopDbContext _context;

        public PieDbRepository(BethanysPieShopDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Pie> AllPies
        {
            get
            {
                return _context.Pies.Include(c => c.Category);
            }
        }

        public void AddPie(Pie pie)
        {
            _context.Pies.Add(pie);
            _context.SaveChanges();
        }

        public void DeletePie(int pieId)
        {
            Pie? pieToDelete = _context.Pies.Find(pieId);
            if (pieToDelete is null) throw new Exception("Pie not found");
            _context.Pies.Remove(pieToDelete);
            _context.SaveChanges();
        }

        public Pie? GetPieById(int pieId)
        {
            return _context.Pies.FirstOrDefault(p => p.PieId == pieId);
        }

        public IEnumerable<Pie> GetPiesOfTheWeek()
        {
            return _context.Pies.Include(c => c.Category).Where(p => p.IsPieOfTheWeek);
        }

        public IEnumerable<Pie> SearchPies(string searchQuery)
        {
            return _context.Pies.Where(p => p.Name.Contains(searchQuery));
        }
    }
}
