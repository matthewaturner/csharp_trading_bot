using System.Threading.Tasks;

namespace Core
{
    public interface IKeyVaultManager
    {
        Task<string> GetSecretAsync(string secretName);
    }
}
