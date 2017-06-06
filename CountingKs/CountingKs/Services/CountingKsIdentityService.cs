using System.Threading;

namespace CountingKs.Services
{
    public class CountingKsIdentityService : ICountingKsIdentityService
    {
        public string CurrentUser
        {
            get { return Thread.CurrentPrincipal.Identity.Name; }
        }
    }
}