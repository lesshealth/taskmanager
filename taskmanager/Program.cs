using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Serilog.Formatting.Compact;


namespace taskmanager
{
    internal class Program
    {
        static List<string> tasks = new List<string>();
        static List<bool> completed = new List<bool>();

        static void Main()
        {
            //Trace.Listeners.Add(new ConsoleTraceListener());
            //Trace.Listeners.Add(new TextWriterTraceListener("taskmanager.log"));
            //Trace.AutoFlush = true;

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File(new CompactJsonFormatter(), "taskmanager.json",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            

            Trace.WriteLine("Менеджер задач запущен");
            Trace.WriteLine($"Время запуска: {DateTime.Now}");

            try
            {
                RunTaskManager();
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Критическая ошибка: {ex.Message}");
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }
            finally
            {
                Trace.WriteLine("Менеджер задач завершил работу");
                Trace.WriteLine("----------------------------------");
            }
        }

        static void RunTaskManager()
        {
            while (true)
            {
                Console.WriteLine("\n1. Добавить задачу");
                Console.WriteLine("2. Удалить задачу");
                Console.WriteLine("3. Показать все задачи");
                Console.WriteLine("4. Отметить как выполненную");
                Console.WriteLine("5. Выход");
                Console.Write("Выберите: ");

                string input = Console.ReadLine();
                Trace.WriteLine($"Пользователь ввел: {input}");

                Console.Clear();

                if (input == "1")
                {
                    AddTask();
                }
                else if (input == "2")
                {
                    DeleteTask();
                }
                else if (input == "3")
                {
                    ShowTasks();
                }
                else if (input == "4")
                {
                    MarkCompleted();
                }
                else if (input == "5")
                {
                    Trace.WriteLine("Завершение работы по команде пользователя");
                    break;
                }
                else
                {
                    Trace.TraceWarning("Неверный ввод в меню");
                    Console.WriteLine("Неверный ввод!");
                }
            }
        }

        static void AddTask()
        {
            Console.Write("Введите задачу: ");
            string task = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(task))
            {
                Trace.TraceWarning("Попытка добавить пустую задачу");
                Console.WriteLine("Задача не может быть пустой!");
                return;
            }

            tasks.Add(task);
            completed.Add(false);
            Trace.TraceInformation($"Добавлена задача: {task}");
            Console.WriteLine("Добавлено!");
        }

        static void DeleteTask()
        {
            if (tasks.Count == 0)
            {
                Trace.TraceWarning("Попытка удаления при пустом списке");
                Console.WriteLine("Нет задач!");
                return;
            }

            ShowTasks();
            Console.Write("Какую удалить? ");

            if (!int.TryParse(Console.ReadLine(), out int index) || index < 1 || index > tasks.Count)
            {
                Trace.TraceError("Ошибка ввода номера задачи для удаления");
                Console.WriteLine("Ошибка!");
                return;
            }

            string deletedTask = tasks[index-1];
            tasks.RemoveAt(index-1);
            completed.RemoveAt(index-1);
            Trace.TraceInformation($"Удалена задача: {deletedTask}");
            Console.WriteLine("Удалено!");
        }

        static void ShowTasks()
        {
            if (tasks.Count == 0)
            {
                Trace.WriteLine("DEBUG: Отображение пустого списка задач");
                Console.WriteLine("Нет задач!");
                return;
            }

            Trace.WriteLine("DEBUG: Отображение списка задач");
            for (int i = 0; i < tasks.Count; i++)
            {
                Console.WriteLine($"{i+1}. {tasks[i]} {(completed[i] ? "[X]" : "[ ]")}");
            }
        }

        static void MarkCompleted()
        {
            if (tasks.Count == 0)
            {
                Trace.TraceWarning("Попытка отметить задачу при пустом списке");
                Console.WriteLine("Нет задач!");
                return;
            }

            ShowTasks();
            Console.Write("Какую отметить? ");

            if (!int.TryParse(Console.ReadLine(), out int index) || index < 1 || index > tasks.Count)
            {
                Trace.TraceError("Ошибка ввода номера задачи для отметки");
                Console.WriteLine("Ошибка!");
                return;
            }

            completed[index-1] = true;
            Trace.TraceInformation($"Задача отмечена как выполненная: {tasks[index-1]}");
            Console.WriteLine("Отмечено!");
        }
    }
}
