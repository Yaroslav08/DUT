using Microsoft.AspNetCore.Identity;

namespace DUT.Infrastructure.IoC.Identity
{
    public class DefaultKeyRing : ILookupProtectorKeyRing
    {
        public static string Current = "Default";
        public string this[string keyId] => keyId;
        public string CurrentKeyId => Current;

        public IEnumerable<string> GetAllKeyIds()
        {
            return new string[] { "Default", "NewPad" };
        }
    }
}
