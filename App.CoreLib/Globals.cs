using System;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using App.CoreLib.Module;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace App.CoreLib
{
    public class Globals
    {
        public static string ExtensionsPath { get; set; }

        public static string ContentRootPath { get; set; }

        public static string ServerPath { get; set; }

        public static void SetServerPath(string serverPath)
        {
            ServerPath = serverPath;
        }

        public static void SetRoot(string contentRootPath, string extensionsPath = "")
        {
            var basePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            ExtensionsPath = extensionsPath;
            ContentRootPath = contentRootPath;
        }

        public static HttpContext CurrentHttpContext { get; set; }

        public static void SetHttpContext(HttpContext context)
        {
            CurrentHttpContext = context;
        }

        public static string CurrentUser
        {

            get
            {
                if (CurrentHttpContext == null)
                    return String.Empty;
                if (CurrentHttpContext.User == null)
                    return String.Empty;

                Claim claim = CurrentHttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (claim == null)
                    return String.Empty;
                return claim.Value;
            }
        }

        public static string DoEverything { get { return "DoEverything"; } }

        public static Func<DateTime> Now = () => DateTimeOffset.Now.DateTime;
    }
}