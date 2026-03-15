using CoreConnect.Shared.Utilities;
using System;
using System.Diagnostics;
using System.IO;

namespace CoreConnect.Agent.Services;

public interface IUninstaller
{
    void UninstallAgent();
}

public class Uninstaller : IUninstaller
{
    public void UninstallAgent()
    {
        if (EnvironmentHelper.IsWindows)
        {
            Process.Start("cmd.exe", "/c sc delete CoreConnect_Service");

            var view = Environment.Is64BitOperatingSystem ?
                "/reg:64" :
                "/reg:32";

            Process.Start("cmd.exe", @$"/c REG DELETE HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\CoreConnect /f {view}");

            var currentDir = Path.GetDirectoryName(typeof(Uninstaller).Assembly.Location);
            Process.Start("cmd.exe", $"/c timeout 5 & rd /s /q \"{currentDir}\"");
        }
        else if (EnvironmentHelper.IsLinux)
        {
            Process.Start("sudo", "systemctl stop coreconnect-agent").WaitForExit();
            Directory.Delete("/usr/local/bin/CoreConnect", true);
            File.Delete("/etc/systemd/system/coreconnect-agent.service");
            Process.Start("sudo", "systemctl daemon-reload").WaitForExit();
        }
        Environment.Exit(0);
    }
}
