using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

#if NETCOREAPP || NETSTANDARD
using Microsoft.Win32;
#endif

public static class SteamFinder
{
    public static string? GetSteamInstallationDirectory()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return GetSteamDirectoryWindows();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return GetSteamDirectoryMac();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return GetSteamDirectoryLinux();
        }

        return null;
    }

    public static List<string> GetSteamLibraryDirectories()
    {
        var steamDir = GetSteamInstallationDirectory();
        var libraryDirs = new List<string>();

        if (steamDir != null && Directory.Exists(steamDir))
        {
            var steamAppsPath = Path.Combine(steamDir, "steamapps");
            if (Directory.Exists(steamAppsPath))
            {
                libraryDirs.Add(steamAppsPath);
            }


            var libraryFoldersVdfPath = Path.Combine(steamAppsPath, "libraryfolders.vdf");

            if (File.Exists(libraryFoldersVdfPath))
            {
                var vdfContent = File.ReadAllText(libraryFoldersVdfPath);
                var regex = new Regex("\"path\"\\s+\"(.+?)\"");
                var matches = regex.Matches(vdfContent);

                foreach (Match match in matches)
                {
                    if (match.Success)
                    {
                        var libraryPath = match.Groups[1].Value.Replace("\\", "\\");
                        var librarySteamApps = Path.Combine(libraryPath, "steamapps");
                        if (Directory.Exists(librarySteamApps))
                        {
                            libraryDirs.Add(librarySteamApps);
                        }
                    }
                }
            }
        }

        return libraryDirs;
    }


    private static string? GetSteamDirectoryWindows()
    {
#if NETCOREAPP || NETSTANDARD
        try
        {
            // For 64-bit systems
            using (var key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Wow6432Node\\Valve\\Steam"))
            {
                var path = key?.GetValue("InstallPath")?.ToString();
                if (path != null)
                {
                    return path;
                }
            }

            // For 32-bit systems
            using (var key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Valve\\Steam"))
            {
                var path = key?.GetValue("InstallPath")?.ToString();
                if (path != null)
                {
                    return path;
                }
            }

            // Fallback to current user
            using (var key = Registry.CurrentUser.OpenSubKey("Software\\Valve\\Steam"))
            {
                return key?.GetValue("SteamPath")?.ToString();
            }
        }
        catch
        {
            // Registry access might be restricted
            return null;
        }
#else
        return null;
#endif
    }

    private static string? GetSteamDirectoryMac()
    {
        var home = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        var steamPath = Path.Combine(home, "Library", "Application Support", "Steam");
        return Directory.Exists(steamPath) ? steamPath : null;
    }

    private static string? GetSteamDirectoryLinux()
    {
        var home = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

        var steamPath1 = Path.Combine(home, ".local", "share", "Steam");
        if (Directory.Exists(steamPath1))
        {
            return steamPath1;
        }

        var steamPath2 = Path.Combine(home, ".steam", "steam");
        if (Directory.Exists(steamPath2))
        {
            return steamPath2;
        }

        return null;
    }
}
