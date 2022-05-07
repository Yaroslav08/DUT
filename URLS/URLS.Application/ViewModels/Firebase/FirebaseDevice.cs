namespace URLS.Application.ViewModels.Firebase
{
    public class FirebaseDevice
    {
        public string Token { get; set; }
        public string Type { get; set; }

        public bool IsAndroid => Type == "Andoid";

        public static FirebaseDevice AsAndroid(string token)
        {
            return new FirebaseDevice
            {
                Token = token,
                Type = "Andoid"
            };
        }

        public static FirebaseDevice AsIOS(string token)
        {
            return new FirebaseDevice
            {
                Token = token,
                Type = "IOS"
            };
        }
    }
}