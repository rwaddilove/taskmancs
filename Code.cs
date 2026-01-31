namespace TaskManCS;

public class Code
{
    public static string InputStr(string prompt, int len)
    {
        Console.Write(prompt);
        string inp = Console.ReadLine()?.Trim() ?? "";  // Handle null input
        Console.ResetColor();
        return inp.Length <= len ? inp : inp.Substring(0, len);
    }

    public static string InputYesNo(string prompt)
    {
        string inp = InputStr(prompt, 1).ToLower();
        return inp == "yes" || inp == "y" ? "Yes" : "";
    }

    public static int InputInt(string prompt, int min, int max)
    {
        Console.Write(prompt);
        int number;
        return int.TryParse(Console.ReadLine(), out number) && number >= min && number <= max  ? number : -9999;
    }

    public static void ViewTaskDetails(List<Task> tasks)
    {
        int id = InputInt("View task ID? ", 0, tasks.Count - 1);
        if (id < 0)
        {
            InputStr("Task not found. Press Enter: ", 1);
            return;
        }
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine($"\n===== TASK {id} DETAILS =====");
        Console.ResetColor();
        DisplayTaskLong(tasks[id]);
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        InputStr("Press Enter...", 1);
        Console.ResetColor();
    }

    public static string InputDate(string prompt)
    {
        string inp = InputStr(prompt, 20);
        if (DateTime.TryParse(inp, out DateTime date))
            return date.ToString("yyyy/MM/dd");   // valid
        return "2099/12/31";                            // invalid
    }

    public static string InputRepeat()      // input repeat option for a task
    {
        string inp = InputStr("Repeat (daily, weekly, 4week, monthly): ", 8).ToLower();
        if (inp.Length == 0) return "";
        if (inp[0] == 'd') return "daily";
        if (inp[0] == 'w') return "weekly";
        if (inp[0] == '4') return "4weeks";
        return "monthly";
    }

    public static void DisplayTaskLong(Task task)   // display one task in full
    {
        Console.WriteLine("1 Title: " + task.Title);
        Console.WriteLine("2 Due: " + (task.Due == "2099/12/31" ? "" : task.Due));
        Console.WriteLine("3 Priority: " + task.Priority);
        Console.WriteLine("4 Repeat: " + task.Repeat);
        Console.WriteLine("5 Label: " + task.Label);
        Console.WriteLine("6 Done: " + task.Done);
        Console.WriteLine("7 Notes: " + task.Notes);
    }

    public static void ListTasks(List<Task> tasks, string filterLabel)  // list tasks one per line
    {
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.Write("ID ");
        Console.Write("Title".PadRight(20));
        Console.Write("Due".PadRight(12));
        Console.Write("Prty".PadRight(5));
        Console.Write("Repeat".PadRight(8));
        Console.Write("Label".PadRight(10));
        Console.Write("Done".PadRight(5));
        Console.WriteLine("Notes");
        Console.ResetColor();
        if (tasks.Count == 0) return;

        for (int i = 0; i < tasks.Count; i++)
        {
            if (filterLabel.Length > 0 && tasks[i].Label != filterLabel) continue;
            if (DateTime.TryParse(tasks[i].Due, out DateTime dueDate) && dueDate < DateTime.Now)
                Console.ForegroundColor = ConsoleColor.Red;  // highlight overdue tasks
            Console.Write(i.ToString("D2").PadRight(3));
            Console.Write(tasks[i].Title.PadRight(20));
            Console.Write(tasks[i].Due == "2099/12/31" ? "".PadRight(12) : tasks[i].Due.PadRight(12));
            Console.Write(tasks[i].Priority.ToString().PadRight(5));
            Console.Write(tasks[i].Repeat.PadRight(8));
            Console.Write(tasks[i].Label.PadRight(10));
            Console.Write(tasks[i].Done.PadRight(5));
            string notes = tasks[i].Notes;
            if (notes.Length < 40)
                Console.WriteLine(notes);
            else
                Console.WriteLine(tasks[i].Notes.Substring(0,37) + "...");
            Console.ResetColor();
        }
    }

    public static void NewTask(List<Task> tasks)        // create a new task
    {
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine("\n===== ADD TASK =====");
        Console.ResetColor();
        tasks.Add(new Task());
        tasks[^1].Title = InputStr("Title: ", 20);  // ^1 is last item (index from end)
        tasks[^1].Due = InputDate("Due (yyyy/mm/dd): ");
        int priority = InputInt("Priority (1/2/3): ", 1, 3);    // returns <0 if invalid
        tasks[^1].Priority = priority < 0 ? 3 : priority;
        tasks[^1].Repeat = InputRepeat();
        tasks[^1].Label = InputStr("Label: ", 12).ToLower();
        tasks[^1].Done = InputYesNo("Done: ");
        tasks[^1].Notes = InputStr("Notes: ", 500);
    }

    public static void EditTask(List<Task> tasks)       // edit a task
    {
        if (tasks.Count == 0) return;
        int id = InputInt("Task to edit ID: ", 0, tasks.Count - 1);
        if (id < 0) {
            InputStr($"Task {id} not found. Press Enter: ", 1);
            return; }

        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine("\n===== EDIT TASK =====");
        Console.ResetColor();
        DisplayTaskLong(tasks[id]);
        int f = InputInt("\nEdit which item? ", 1, 7);
        if (f < 0) return;

        if (f == 1) tasks[id].Title = InputStr("Title: ", 20);
        if (f == 2) tasks[id].Due = InputDate("Due (yyyy/mm/dd): ");
        if (f == 3)
        {
            int priority = InputInt("Priority (1/2/3): ", 1, 3); // returns <0 if out of range
            tasks[id].Priority = priority < 0 ? 3 : priority;
        }
        if (f == 4) tasks[id].Repeat = InputRepeat();       // **** CHECK THAT THERE IS A DUE DATE?
        if (f == 5) tasks[id].Label = InputStr("Label: ", 12).ToLower();
        if (f == 6) tasks[id].Done = InputYesNo("Done: ");
        if (f == 7) tasks[id].Notes = InputStr("Notes: ", 500);
        InputStr($"Task {id} updated. Press Enter: ", 1);

    }

    public static void SetDone(List<Task> tasks)
    {
        if (tasks.Count == 0) return;
        int id = InputInt("Task ID to mark done: ", 0, tasks.Count - 1);
        if (id < 0) return;
        if (tasks[id].Repeat == "") return;     // not a repeating task

        // must be a repeating task, get new due date and clear Done
        if (tasks[id].Due == "2099/12/31" || !DateTime.TryParse(tasks[id].Due, out DateTime date))
            return;  // invalid date
        if (tasks[id].Repeat == "daily") date = date.AddDays(1);
        if (tasks[id].Repeat == "weekly") date = date.AddDays(7);
        if (tasks[id].Repeat == "4weekly") date = date.AddDays(28);
        if (tasks[id].Repeat == "monthly") date = date.AddMonths(1);
        tasks[id].Due = date.ToString("yyyy/MM/dd");
        tasks[id].Done = "";
        InputStr("Repeat task: New due date set. Press Enter...", 1);
    }

    public static void RemoveTask(List<Task> tasks)
    {
        if (tasks.Count == 0) return;

        int id = InputInt("Enter Task ID to remove: ", 0, tasks.Count - 1);
        if (id < 0)
        {
            InputStr("Task not found. Press Enter: ", 1);
            return;
        }
        tasks.RemoveAt(id);
        InputStr($"Task {id} removed. Press Enter...", 1);
    }

    public static void SortTasks(List<Task> tasks)
    {
        string sortType = InputStr("Sort by (d)ate, (p)riority, (l)able: ", 10).ToLower();
        if (sortType.Length == 0) sortType = "d";
        if (sortType[0] == 'd') tasks.Sort((a, b) => a.Due.CompareTo(b.Due));
        if (sortType[0] == 'p') tasks.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        if (sortType[0] == 'l') tasks.Sort((a, b) => a.Label.CompareTo(b.Label));
    }
}
