namespace Ecomm.UI.Common
{
    public static class SD
    {
        //Roles
        public const string Role_Admin = "Admin";
        public const string Role_Individual = "Individual";

        //API Paths
        public static string AuthBaseUrl { get; set; }
        public static string BookBaseUrl { get; set; }
        public static string AuthAPIPath => $"{AuthBaseUrl}/api/Auth";
        public static string BookAPIPath => $"{BookBaseUrl}/api/Book";
        public static string CategoryAPIPath => $"{BookBaseUrl}/api/Category";
        public static string CoverTypeAPIPath => $"{BookBaseUrl}/api/CoverType";
        //public static string CartAPIPath => $"{APIBaseUrl}/api/Cart";
    }
}