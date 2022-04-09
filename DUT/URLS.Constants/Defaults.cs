namespace URLS.Constants
{
    public static class Defaults
    {
        public const string Password = "Admin01!";
        public const string CreatedBy = "system";
        public const string IP = "::1";
        public const string GroupImage = "https://image.flaticon.com/icons/png/512/25/25437.png";

        public static DateTime GroupInviteActiveFrom = DateTime.Now;
        public static DateTime GroupInviteActiveTo = DateTime.Now.AddMonths(2);

    }
}