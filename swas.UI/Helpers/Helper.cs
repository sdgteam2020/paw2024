using ASPNetCoreIdentityCustomFields.Data;
using Org.BouncyCastle.Utilities;
using swas.BAL.DTO;

namespace swas.UI.Helpers
{
    public static class Helper
    {
        public static string LoginDetails(Login Logins)
        {
            return Logins.Rank.Trim() + " " + Logins.Offr_Name.Trim() + " / " + Logins.UserName.Trim() + "";
        }
        public static string UserInfoDetails(ApplicationUser User)
        {
            return User.Rank.Trim() + " " + User.Offr_Name.Trim() + " / " + User.UserName.Trim() + "";
        }
    }
}
