using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.CoreLib.EF.Data.Entity;

namespace App.CoreLib.EF.Messages
{
  public class EntityResult
  {
    public EntityResult()
    {
      Succeeded = true;
    }
    private static readonly EntityResult _success = new EntityResult { Succeeded = true };
    private List<EntityError> _errors = new List<EntityError>();

    public bool Succeeded { get; protected set; }

    public EntityBase Entity { get; set; }
    public IEnumerable<EntityError> Errors => _errors;

    public static EntityResult Success => _success;

    public static EntityResult Failed(params EntityError[] errors)
    {
      var result = new EntityResult { Succeeded = false };
      if (errors != null)
      {
        result._errors.AddRange(errors);
      }
      return result;
    }
    public override string ToString()
    {
      return Succeeded ?
             "Succeeded" :
             string.Format("{0} : {1}", "Failed", string.Join(",", Errors.Select(x => x.Code).ToList()));
    }
  }
  public class EntityError
  {
    public string Code { get; set; }
    public string Description { get; set; }
  }
}
