# Table of contents
* [Host ASP.NET Core on Windows with IIS](#Host-ASP.NET-Core-on-Windows)


# Host ASP.NET Core on Windows

1. Install the .NET Core Windows Server Hosting bundle
> * Navigate to the `.NET All Downloads page.`
> *  Select the latest non-preview .NET Core runtime from the list (`.NET Core > Runtime > .NET Core Runtime x.y.z`).
> * On the .NET Core runtime download page under Windows, select the **Server Hosting Installer** link to download the `.NET Core Windows Server Hosting` bundle.
2. Restart the system or execute `net stop was /y `followed by `net start w3svc`

