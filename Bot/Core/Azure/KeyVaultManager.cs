using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Core.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Core.Azure
{
    public class KeyVaultManager : IKeyVaultManager
    {
        KeyVaultConfiguration config;
        SecretClient client;

        public KeyVaultManager(IOptions<KeyVaultConfiguration> config)
        {
            this.config = config.Value;
            /*this.client = new KeyVaultClient(
                new KeyVaultClient.AuthenticationCallback(
                    new AzureServiceTokenProvider().KeyVaultTokenCallback));*/

            var options = new DefaultAzureCredentialOptions();
            options.ExcludeSharedTokenCacheCredential = true;
            client = new SecretClient(
                vaultUri: new Uri(config.Value.BaseUrl),
                credential: new DefaultAzureCredential(options));
        }

        public async Task<string> GetSecretAsync(string secretName)
        {
            var bundle = await client.GetSecretAsync(secretName);

            return bundle.Value.Value;
        }
    }
}
