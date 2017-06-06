using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountingKs.Services
{
    public interface ICountingKsIdentityService
    {
       
            string CurrentUser { get; }
       
    }
}
