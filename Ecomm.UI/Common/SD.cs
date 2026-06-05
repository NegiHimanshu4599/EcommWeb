namespace Ecomm.UI.Common
{
    public static class SD
    {
        //Roles
        public const string Role_Admin = "Admin";
        public const string Role_Individual = "Individual";

        // Sessions
        public const string Ss_CartSessionCount = "Cart Count Session";

        //API Paths
        public static string AuthBaseUrl { get; set; }
        public static string BookBaseUrl { get; set; }
        public static string CartBaseUrl { get; set; }
        // Auth Path
        public static string AuthAPIPath => $"{AuthBaseUrl}/api/Auth";
        //Books Path
        public static string BookAPIPath => $"{BookBaseUrl}/api/Book";
        public static string CategoryAPIPath => $"{BookBaseUrl}/api/Category";
        public static string CoverTypeAPIPath => $"{BookBaseUrl}/api/CoverType";
        //Cart Path
        public static string CartAPIPath => $"{CartBaseUrl}/api/Cart";
    }
}