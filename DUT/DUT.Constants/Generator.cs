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
    }
}