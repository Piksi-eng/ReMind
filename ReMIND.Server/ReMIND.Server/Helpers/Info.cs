namespace ReMIND.Server.Helpers
{
    public static class Info
    {
        public const string Unknown = "Oops! An unknown error happened";

        public const string NoPermission = "You don't have permission";
        public const string NoPrivileges = "You don't have any privileges";
        public const string NoSession = "Your session has expired";
        public const string NoObject = "The object doesn't exist";
        public const string Deleted = " has been deleted";

        //Person 
        public const string BadPassword = "Wrong password entered";
        public const string LoggedOut = "You have been logged out";
        public const string LoggedIn = "You have been logged in";
        public const string EmailUsed = "Email is already in use";
        public const string PhoneUsed = "Phone is already in use";
        public const string ResetPassword = "The password has been reset";
        public const string WrongEmail = "Wrong email";
        public const string WrongPassword = "Wrong password";
        public const string AuthWrong = "Error with authentication";
        public const string Unemployed = "Account is unemployed"; 

        //Team  
        public const string TeamNameUsed = "The Team Name you entered is already in use";

        //JobTag
        public const string JobTagNameColorUsed = "The Job Tag Name or Color you entered is already in use";
        public const string JobTagNameUsed = "The Job Tag Name you entered is already in use";
        public const string JobTagColorUsed = "The Job Tag Color you entered is already in use";

        //Job
        public const string SuccessArchive = "Job(s) archived successfully";
        public const string SuccessWelcome = "The Welcome Message has been sent";
        
        //TeamLink
        public const string TeamLinkUsed = "The Team Link you entered is already in use";

        //Archive
        public const string IndexOutOfRange = "Pst, index is out of range";
        public const string ReccuringTypeError = "Error selecting reccuring type";
        

    }
}