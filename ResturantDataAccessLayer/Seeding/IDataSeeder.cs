using System.Threading.Tasks;

namespace ResturantDataAccessLayer.Seeding
{
    public interface IDataSeeder
    {
        Task SeedAsync();
    }
}
