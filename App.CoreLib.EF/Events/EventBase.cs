using System;

namespace App.CoreLib.EF.Events
{
  public class EventBase
  {
    public EventBase()
    {
      OccuredOn = DateTime.Now;
    }

    protected DateTime OccuredOn
    {
      get;
      set;
    }
  }
}