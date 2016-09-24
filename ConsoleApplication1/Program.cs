using System;
using System.IO;
using System.Security.Permissions;

public class Watcher
{

    public static void Main()
    {
        Run();
    }

    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public static void Run()
    {
        string path = "C:\\ProgramData\\4-Tell2.0";

        // Create a new FileSystemWatcher and set its properties.
        FileSystemWatcher watcher = new FileSystemWatcher();
        watcher.Path = path;
        watcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite;
        watcher.Filter = "SiteRules.xml";
        watcher.IncludeSubdirectories = true;

        // Add event handlers.
        watcher.Changed += new FileSystemEventHandler(OnChanged);
        //watcher.Created += new FileSystemEventHandler(OnChanged);
        //watcher.Deleted += new FileSystemEventHandler(OnChanged);
        watcher.Renamed += new RenamedEventHandler(OnRenamed);

        // Begin watching.
        watcher.EnableRaisingEvents = true;

        // Wait for the user to quit the program.
        Console.WriteLine("Press \'q\' to quit the sample.");
        while (Console.Read() != 'q') ;
    }

    // Define the event handlers.
    private static void OnChanged(object source, FileSystemEventArgs e)
    {
        // Specify what is done when a file is changed, created, or deleted.
        Console.WriteLine("File: " + e.FullPath + " " + e.ChangeType);
        string[] pathParts = e.FullPath.Split('\\');
        string clientAlias = pathParts[3];
        string machineName = Environment.MachineName;

        //string destPath = string.Format("\\\\boostlivegroupdiag.file.core.windows.net\\SiteRulesBackups\\{0}\\", machineName, clientAlias);
        string destPath = string.Format("C:\\Temp\\SiteRulesBackups\\{0}\\{1}", machineName, clientAlias);

        Directory.CreateDirectory(destPath);
        string date = DateTime.Now.ToString("MM-dd-yy hh-mm-ss.fff");
        date = date.Replace(':', '.');
        string destFilePath = Path.Combine(destPath, string.Format("SiteRules - {0}.xml", date));
        File.Copy(e.FullPath, destFilePath);
    }

    private static void OnRenamed(object source, RenamedEventArgs e)
    {
        // Specify what is done when a file is renamed.
        Console.WriteLine("File: {0} renamed to {1}", e.OldFullPath, e.FullPath);
    }
}