using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using App.CoreLib.EF.Data.Entity;

namespace App.CoreLib.EF.Data.Identity
{
    public class AppRole : IdentityRole, IEntity
    {
        public string Product { get; set; }

        public virtual ICollection<AppUserRole> UserRoles { get; set; }
        public virtual ICollection<AppRoleClaim> RoleClaims { get; set; }
        public int RowVersion { get; set; }
    }
}
