using System.Threading.Tasks;

namespace ArguSzem.Services
{
    public interface IDataStore
    {
        bool IsUserLoggedIn { get; }
        Task<bool> RegisterAsync(string email, string password, string confirmPassword);
        Task<bool> LoginAsync(string name, string password);
        Task<bool> LogoutAsync();
        Task<int> GetPortAsync();
    }
}
