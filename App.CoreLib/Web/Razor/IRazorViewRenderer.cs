using System.Threading.Tasks;

namespace App.CoreLib.Web.Razor
{
    public interface IRazorViewRenderer
    {
        Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model);
    }
}