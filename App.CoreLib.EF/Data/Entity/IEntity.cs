using System;

namespace App.CoreLib.EF.Data.Entity
{
    public interface IEntity
    {
        int RowVersion { get; set; }
    }
    public interface IdentityEntity : IEntity
    {
        int IdentityId { get; set; }
    }
    public interface IAuditEntity : IEntity
    {
        string CreatedById { get; set; }
        DateTime CreatedDateTime { get; set; }
        string ChangedById { get; set; }
        DateTime ChangedDateTime { get; set; }
    }
}