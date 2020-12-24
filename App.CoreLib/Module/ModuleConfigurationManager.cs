using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace App.CoreLib.Module
{
    public class ModuleConfigurationManager
    {
        public static readonly string ModulesFilename = "modules.json";

        public static IEnumerable<ModuleInfo> GetModules()
        {
            var modulesPath = Path.Combine(Globals.ContentRootPath, ModulesFilename);
            using var reader = new StreamReader(modulesPath);
            string content = reader.ReadToEnd();
            dynamic modulesData = JsonConvert.DeserializeObject(content);
            foreach (dynamic module in modulesData)
            {
                yield return new ModuleInfo
                {
                    Id = module.id,
                    IsBundledWithHost = module.isBundledWithHost
                };
            }
        }
    }
}