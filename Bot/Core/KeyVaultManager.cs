
using Core.Configuration;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Core
{
    public class KeyVaultManager : IKeyVaultManager
    {
        KeyVaultConfiguration config;
        KeyVaultClient client;

        public KeyVaultManager(IOptions<KeyVaultConfiguration> config)
        {
            this.config = config.Value;
            this.client = new KeyVaultClient(
                new KeyVaultClient.AuthenticationCallback(
                    new AzureServiceTokenProvider().KeyVaultTokenCallback));
        }

        public async Task<string> GetSecretAsync(string secretName)
        {
            SecretBundle bundle = await client.GetSecretAsync(
                vaultBaseUrl: config.BaseUrl,
                secretName: secretName);

            return bundle.Value;
        }
    }
}
