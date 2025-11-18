namespace swas.BAL.Utility
{
    ///Created and Reviewed by : Sub Maj Sanal
    ///Reviewed Date : 30 Jul 23
    ///Tested By :- 
    ///Tested Date : 
    ///Start
    public class EnumHelper
    {
        public enum Response
        {
            Success = 1,
            Default = 0,
            Error = -1,
            Mandatory = -2,
            SessionExpired = -3,
            InvalidCredentials = -4,
            Alreadyapproved = -5,
            Prviousnotapproved = -6
            
        }
        public enum SaveData
        {
             Save = 1,
             Update = 2,
             Delete = 3,
             Duplicate = 4



        }
        public static class StatusNames
        {
            public static readonly Dictionary<int, string> Map = new Dictionary<int, string>
    {
        { 6, "ASDC Vetting" },
        { 7, "ACG Vetting" },
        { 11, "AHCC Vetting" },
        { 20, "Auto Committee (DGIS)" },
        { 21, "IPA Stage (DDGIT)" },
        { 24, "Arch Vetting (AHCC)" },
        { 25, "Lab Test (ACG)" },
        { 26, "IAM Integ (AHCC)" },
        { 27, "Remote Test (ACG)" },
        { 28, "MI-11 Clearance" },
        { 29, "Whitelisting Completed (DDGIT)" }
    };
        }

    }
}
