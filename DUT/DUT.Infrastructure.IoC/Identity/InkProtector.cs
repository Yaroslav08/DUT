using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DUT.Infrastructure.IoC.Identity
{
    public class InkProtector : ILookupProtector
    {
        public InkProtector() { }

        public string Unprotect(string keyId, string data)
            => data?.Substring(4);

        public string Protect(string keyId, string data)
            => "ink:" + data;
    }
}
