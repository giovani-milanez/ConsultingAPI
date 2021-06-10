using Database.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Extension
{
    public static class UserExtensions
    {
        public static bool IsClient(this User @this)
        {
            return !@this.IsConsultant;
        }
        public static bool IsConsultantOrAdmin(this User @this)
        {
            return @this.IsConsultant || @this.IsAdmin;
        }

        public static bool IsClientOrAdmin(this User @this)
        {
            return !@this.IsConsultant || @this.IsAdmin;
        }
    }
}
