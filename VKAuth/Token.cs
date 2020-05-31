namespace VKAuth
{
    class Token
    {
        public static void Clear()
        {
            Properties.Settings.Default.Reset();
        }
        public static void Set(string token)
        {
            Properties.Settings.Default.Token = token;
            Properties.Settings.Default.Save();
        }
        public static string Get()
        {
            return Properties.Settings.Default.Token;
        }
    }
}
