using System.Collections.Generic;
using App.CoreLib.EF.Data.Entity;

namespace App.CoreLib.EF.Events
{
    public class AfterSaveEvent : EventBase
    {
        public AfterSaveEvent()
        {
            Items = new List<EventEntity>();
        }

        public List<EventEntity> Items { get; set; }
    }
}