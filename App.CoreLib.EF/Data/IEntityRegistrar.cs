using Microsoft.EntityFrameworkCore;

namespace App.CoreLib.EF.Data
{
    public interface IEntityRegistrar
    {
        void RegisterEntities(ModelBuilder modelBuilder);
    }
}