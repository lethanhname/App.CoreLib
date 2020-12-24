using System;
using System.Collections.Generic;
using System.Text;

namespace App.CoreLib.EF.Messages
{
    public class GenericErrorDescriber
    {
        public virtual EntityError DefaultError(string message)
        {
            return new EntityError
            {
                Code = nameof(DefaultError),
                Description = message
            };
        }
    }
}
