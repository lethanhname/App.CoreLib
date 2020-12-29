using System.Collections.Generic;
using App.CoreLib.EF.Data.Entity;
using App.CoreLib.EF.Messages;
using Microsoft.EntityFrameworkCore;

namespace App.CoreLib.EF.Events
{
    public class EventEntity
    {
        public EventEntity()
        {
            Messages = new List<EntityError>();
        }
        public string EntityType { get; set; }
        public EntityState State { get; set; }

        public IEntity EntityData { get; set; }

        public List<EntityError> Messages { get; set; }
    }
}