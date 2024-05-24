using Org.BouncyCastle.Utilities;
using swas.BAL.DTO;

namespace swas.UI.Helpers
{
    public static class Helper
    {
        public static string LoginDetails(Login Logins)
        {
            return Logins.Unit.Trim() +"("+Logins.Rank.Trim() + "" + Logins.Offr_Name.Trim()+" / "+Logins.UserName.Trim() + ")";
        }
    }
}
