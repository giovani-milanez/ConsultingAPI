using Database.Enum;
using Database.Model;

namespace Database.Extension
{
    public static class UserExtensions
    {
        public static bool IsAdmin(this User @this)
        {
            return @this.Type == AccountType.ADMIN;
        }
        public static bool IsClient(this User @this)
        {
            return @this.Type == AccountType.CLIENT;
        }
        public static bool IsConsultant(this User @this)
        {
            return @this.Type == AccountType.CONSULTANT;
        }
        public static bool IsConsultantOrAdmin(this User @this)
        {
            return @this.Type == AccountType.CONSULTANT || @this.Type == AccountType.ADMIN;
        }
        public static bool IsClientOrAdmin(this User @this)
        {
            return @this.Type == AccountType.CLIENT || @this.Type == AccountType.ADMIN;
        }
    }
}
