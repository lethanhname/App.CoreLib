using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace App.CoreLib.EF.Data.Entity
{
    public abstract class EntityBase : IEntity
    {
        public virtual int RowVersion { get; set; }

        public virtual bool IsValid()
        {
            return Validate().Count == 0;
        }

        public virtual IList<ValidationResult> Validate()
        {
            IList<ValidationResult> validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(this, new ValidationContext(this, null, null), validationResults, true);
            return validationResults;
        }
    }
    public abstract class IdentityEntityBase : EntityBase, IdentityEntity
    {
        public virtual int IdentityId { get; set; }
    }
    public abstract class AuditEntityBase : EntityBase, IAuditEntity
    {
        public virtual string CreatedById { get; set; }
        public virtual DateTime CreatedDateTime { get; set; }
        public virtual string ChangedById { get; set; }
        public virtual DateTime ChangedDateTime { get; set; }
    }

    public static class EntityExtension
    {
        public static void Merge<TRecord, T>(this TRecord record, T request)
            where TRecord : class
            where T : class
        {
            foreach (var prop in typeof(TRecord).GetProperties())
            {
                if (prop.CanWrite)
                {
                    var requestProp = request.GetType().GetProperty(prop.Name);

                    if (requestProp != null && requestProp.PropertyType == prop.PropertyType)
                    {
                        if (requestProp.CanRead)
                        {
                            prop.SetValue(record, requestProp.GetValue(request));
                        }
                    }
                }
            }
        }
    }
}