using System.Text;

namespace DUT.Constants
{
    public static class Generator
    {
        private static string _upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static string _lowerChars = "abcdefghijklmnopqrstuvwxyz";
        private static string _numbersChars = "0123456789";
        private static string _chars = $"{_upperChars}{_lowerChars}{_numbersChars}";
        public static string CreateGroupInviteCode()
        {
            int lengthOfSequence = 4;
            StringBuilder sb = new StringBuilder();
            
            var random = new Random();

            for (int i = 0; i < lengthOfSequence; i++)
            {
                sb.Append(_chars[random.Next(_chars.Length)]);
            }

            sb.Append("#");

            for (int i = 0; i < lengthOfSequence; i++)
            {
                sb.Append(_numbersChars[random.Next(_numbersChars.Length)]);
            }

            return sb.ToString();
        }

        public static string CreateAppId()
        {
            return GetUniqCode(4);
        }

        public static string CreateAppSecret()
        {
            return GetString(70);
        }

        public static string GetUsername()
        {
            string username;
            username = GetString(30);
            return username;
        }

        public static string GetUniqCode(int sections)
        {
            var commonWords = sections * 4;
            var commonCountOfSymbols = commonWords + (sections - 1);
            var stringChars = new char[commonCountOfSymbols];
            var positons = GetHyphenPositions(sections);
            var random = new Random();
            for (int i = 0; i < stringChars.Length; i++)
            {
                if (positons.Contains(i))
                {
                    stringChars[i] = '-';
                    continue;
                }
                stringChars[i] = _chars[random.Next(_chars.Length)];
            }
            return new String(stringChars);
        }

        private static int[] GetHyphenPositions(int sections)
        {
            var pos = new int[sections - 1];
            var baseIndex = 4;
            for (int i = 0; i < pos.Length; i++)
            {
                pos[i] = baseIndex;
                baseIndex += 4 + 1;
            }
            return pos;
        }

        public static string GetString(int length, bool IsUpper = false, bool IsLowwer = false)
        {
            var stringChars = new char[length];
            var random = new Random();
            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = _chars[random.Next(_chars.Length)];
            }
            var result = new string(stringChars);
            return IsUpper ? result.ToUpper() : IsLowwer ? result.ToLower() : result;
        }
    }
}