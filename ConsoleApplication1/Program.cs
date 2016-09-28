using System;
using System.Diagnostics;
using System.IO;
using System.Security.Permissions;

public class Watcher
{
    const string appName = "Site Rules Backup";

    public static void Main()
    {
        Log(appName + " is running");
        Run();
    }

    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public static void Run()
    {
        try
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

            // Begin watching.
            watcher.EnableRaisingEvents = true;

            // Wait for the user to quit the program.
            Console.WriteLine("Press \'q\' to quit.");

            while (Console.Read() != 'q') ;
        }
        catch (Exception e)
        {
            Log(e.Message, EventLogEntryType.Error);
        }
        finally
        {
            Log(appName + " shut down");
        }
    }

    private static void Log(string message, EventLogEntryType type = EventLogEntryType.Information)
    {
        string source = "4-Tell";
        string logName = "Applications and Services";

        if (!EventLog.SourceExists(source))
            EventLog.CreateEventSource(source, logName);

        EventLog.WriteEntry(source, message, type);
        Console.WriteLine(message);
        //EventLog.WriteEntry(sSource, sEvent,
        //    EventLogEntryType.Warning, 234);
    }

    // Define the event handlers.
    private static void OnChanged(object source, FileSystemEventArgs e)
    {
        try
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
        catch (Exception ex)
        {
            Log(ex.Message, EventLogEntryType.Error);
        }
    }
}