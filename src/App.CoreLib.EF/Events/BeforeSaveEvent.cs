using System.Collections.Generic;
using App.CoreLib.EF.Data.Entity;

namespace App.CoreLib.EF.Events
{
    public class BeforeSaveEvent : EventBase
    {
        public BeforeSaveEvent()
        {
            Items = new List<EventEntity>();
        }

        public List<EventEntity> Items { get; set; }
    }
}