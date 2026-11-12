namespace BethanysPieShop.Models
{
    public interface IPieRepository
    {
        IEnumerable<Pie> AllPies { get; }
        Pie? GetPieById(int pieId);
        IEnumerable<Pie> GetPiesOfTheWeek();

        IEnumerable<Pie> SearchPies(string searchQuery);
        
        void AddPie(Pie pie);
        void DeletePie(int pieId);
        
    }
}
