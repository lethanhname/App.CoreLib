using System.Threading.Tasks;

namespace App.CoreLib.EF.Events
{
    public interface IEventHandler<T> where T : EventBase
    {
        Task RunAsync(T obj);
    }
}