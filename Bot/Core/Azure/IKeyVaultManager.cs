using System.Threading.Tasks;

namespace Core.Azure
{
    public interface IKeyVaultManager
    {
        Task<string> GetSecretAsync(string secretName);
    }
}
