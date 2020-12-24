using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using App.CoreLib.EF.Data.Entity;

namespace App.CoreLib.EF.Data.Identity
{
    public class AppUser : IdentityUser, IEntity
    {
        public string GivenName { get; set; }
        public string FamilyName { get; set; }


        public virtual ICollection<AppUserClaim> Claims { get; set; }
        public virtual ICollection<AppUserLogin> Logins { get; set; }
        public virtual ICollection<AppUserToken> Tokens { get; set; }
        public virtual ICollection<AppUserRole> UserRoles { get; set; }

        public int RowVersion { get; set; }
    }
}