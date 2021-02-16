using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace ArguSzem.Models
{
    public static class TokenHandlerModel
    {
        private static ISettings AppSettings => CrossSettings.Current;

        public static string Username
        {
            get => AppSettings.GetValueOrDefault("Username", "");
            set => AppSettings.AddOrUpdateValue("Username", value);
        }
        public static string AccessToken
        {
            get => AppSettings.GetValueOrDefault("AccessToken", "");
            set => AppSettings.AddOrUpdateValue("AccessToken", value);
        }
        public static int Port
        {
            get => AppSettings.GetValueOrDefault("Port", 0);
            set => AppSettings.AddOrUpdateValue("Port", value);
        }
    }
}
