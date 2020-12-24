using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using App.CoreLib.EF.Data.Entity;

namespace App.CoreLib.EF.Data.Identity
{
    public class AppUserToken : IdentityUserToken<string>
    {
    }
}
