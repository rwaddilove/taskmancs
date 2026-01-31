namespace TaskManCS;

public class FileOp
{

    public static void CreateConfig(bool overwrite)     // Create TaskManConfig.txt in user's home folder
    {
        string homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string configPath = Path.Combine(homePath, "TaskManConfig.txt");
        if (File.Exists(configPath) && !overwrite)      // don't overwrite existing file?
            return;

        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine("===== Creating config =====");
        string dataPath = Code.InputStr("Path to TaskMan folder? ", 255);
        Console.ResetColor();
        
        if (dataPath.EndsWith(Path.DirectorySeparatorChar))     // don't let user add trailing / or \
            dataPath = dataPath.Substring(0, dataPath.Length - 1);
        if (!Directory.Exists(dataPath))
        {
            dataPath = homePath;    // default to user's home folder if no or bad folder
            Console.WriteLine("Folder not found, using: " + homePath);
        }

        // create the config file
        string[] config = [dataPath,                // path to data folder - dataFolder
            Path.Combine(dataPath,"TaskMan.txt"),   // path to data file - dataFile
            "" ];                                   // future expansion?
        File.WriteAllLines(Path.Combine(homePath, "TaskManConfig.txt"), config);
    }


    public static string ReadConfig(string setting)     // read a setting from the config file
    {
        string homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string[] config;
        try
        {
            config = File.ReadAllLines(Path.Combine(homePath, "TaskManConfig.txt"));
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return "";
        }

        if (config.Length < 2) return "";
        if (setting == "dataFolder") return config[0];
        if (setting == "dataFile") return config[1];
        return "";
    }


    public static void WriteTasks(List<Task> tasks)     // save all tasks in csv format
    {
        string filePath = ReadConfig("dataFile");
        if (filePath.Length < 2) return;

        using (StreamWriter writer = new StreamWriter(filePath))    // disposes when done
        {
            foreach (Task task in tasks)
            {
                string t = "\"" +task.Title + "\"," +
                    "\"" + task.Due +"\"," +
                    "\"" + task.Priority + "\"," +
                    "\"" + task.Repeat +"\"," +
                    "\"" + task.Label +"\"," +
                    "\"" + task.Done +"\"," +
                    "\"" + task.Notes + "\"";
                writer.WriteLine(t);
            }
        }
    }


    public static void ReadTasks(List<Task> tasks)      // read csv file into tasks[] list
    {
        tasks.Clear();
        string filePath = ReadConfig("dataFile");
        if (filePath.Length < 2) return;

        string[] readText;
        try
        {
            readText = File.ReadAllLines(filePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Read tasks: " + ex.Message);
            return;
        }

        // data file is a csv file, one task per line. Remove quotes, split into task properties
        foreach (string s in readText)
        {
            if (s.Length < 10) continue;    // too small, something wrong
            string csvTask = s.Trim().Substring(1, s.Length - 2);   // remove first and last quote
            string[] task = csvTask.Split("\",\"");
            int last = tasks.Count;         // becomes last task index when new task added
            tasks.Add(new Task());
            tasks[last].Title = task[0];
            tasks[last].Due = task[1];
            tasks[last].Priority = int.Parse(task[2]);
            tasks[last].Repeat =  task[3];
            tasks[last].Label = task[4];
            tasks[last].Done = task[5];
            tasks[last].Notes = task[6];
        }
    }

}