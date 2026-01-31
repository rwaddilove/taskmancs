// TaskManCS tasks/to-do in C# by R.A.Waddilove (github.com/rwaddilove)
// A simple console app that runs from the command prompt. Public domain.
//
// Notes: It was written to learn C# programming. As I've only just,
// started, there may be better ways to do things. I use dark mode.
// You'll need to pick different colors in light mode.
// Not vibe coded, all mistakes are my own!

namespace TaskManCS
{

    public class Task
    {
        public string Title = "";
        public string Due = "";         // eg. 2026/04/23
        public int Priority = 3;        // 1-3
        public string Repeat = "";      // daily, weekly, 4week, monthly
        public string Label = "";
        public string Done = "";        // Yes or ""
        public string Notes = "";
    }

    class Program
    {
        public static void Main(string[] args)
        {
            List<Task> tasks = new List<Task>(); // list of task objects
            Console.Clear();
            FileOp.CreateConfig(overwrite: false); // create if not found
            FileOp.ReadTasks(tasks);

            string filterLabel = "";
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("== TaskManCS: Task Manager ==========================================\n");
                Console.ResetColor();
                Code.ListTasks(tasks, filterLabel);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                string input = Code.InputStr("OPTIONS: New, Edit, View, Done, Sort, Filter, Remove, Quit? ", 10);
                Console.ResetColor();
                if (input.Length == 0) continue;
                if (input[0] == 'n') Code.NewTask(tasks);
                if (input[0] == 'e') Code.EditTask(tasks);
                if (input[0] == 'v') Code.ViewTaskDetails(tasks);
                if (input[0] == 'd') Code.SetDone(tasks);
                if (input[0] == 's') Code.SortTasks(tasks);
                if (input[0] == 'f') filterLabel = Code.InputStr("Filter by label: ", 10).ToLower();
                if (input[0] == 'r') Code.RemoveTask(tasks);
                if (input[0] == 'q') break;
            }
            FileOp.WriteTasks(tasks);
        }
    }
}
