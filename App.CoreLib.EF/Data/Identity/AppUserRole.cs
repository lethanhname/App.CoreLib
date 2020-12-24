using Microsoft.AspNetCore.Identity;
using System;
using App.CoreLib.EF.Data.Entity;

namespace App.CoreLib.EF.Data.Identity
{
    public class AppUserRole : IdentityUserRole<string>, IEntity
    {
        public int RowVersion { get; set; }
    }
}