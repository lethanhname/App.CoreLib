using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using App.CoreLib.EF.Context;
using App.CoreLib.EF.Messages;

namespace App.CoreLib.EF
{
    public interface IStorage
    {
        IStorageContext StorageContext { get; }

        Task<EntityResult> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
