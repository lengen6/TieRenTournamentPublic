using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace TieRenTournament.Services
{
    public class AuthMessageSenderOptions
    {
        public string? SendGridKey { get; set; }

    }

}
