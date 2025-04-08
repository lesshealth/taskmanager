using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Json;


namespace taskmanager
{
    internal class Program
    {

        public static class Tracer
        {
            public static TraceSource TaskManagerTrace = new TraceSource("TaskManagerTrace");
        }
        static List<string> tasks = new List<string>();
        static List<bool> completed = new List<bool>();

        static void Main()
        {
            Tracer.TaskManagerTrace.Listeners.Add(new TextWriterTraceListener("logs\\taskmanagerTrace.log"));
            Tracer.TaskManagerTrace.Flush();

            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File(
                formatter: new JsonFormatter(),
                path: "logs\\taskmanager.json",
                rollingInterval: RollingInterval.Day)
            .CreateLogger();


            Log.Information("Менеджер задач запущен");

            //Trace.WriteLine("Менеджер задач запущен");
            //Trace.WriteLine($"Время запуска: {DateTime.Now}");

            try
            {
                RunTaskManager();
            }
            catch (Exception ex)
            {
                Log.Error($"Критическая ошибка: {ex.Message}");
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }
            finally
            {
                Log.Information("Менеджер задач завершил работу");
                Log.Information("----------------------------------");
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
                Log.Information($"Пользователь ввел: {input}");

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
                    Log.Information("Завершение работы по команде пользователя");
                    break;
                }
                else
                {
                    Log.Warning("Неверный ввод в меню");
                    Log.Information("Неверный ввод!");
                }
            }
        }

        static void AddTask()
        {
            Tracer.TaskManagerTrace.TraceEvent(TraceEventType.Start, 0, "Начало AddTask");
            Stopwatch sw = Stopwatch.StartNew();
            Console.Write("Введите задачу: ");
            string task = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(task))
            {
                Tracer.TaskManagerTrace.TraceEvent(
                    TraceEventType.Warning,
                    2,
                    "Попытка добавить задачу с пустым названием.");
                Log.Warning("Попытка добавить пустую задачу");
                Log.Information("Задача не может быть пустой!");
                return;
            }

            tasks.Add(task);
            completed.Add(false);
            Log.Information($"Добавлена задача: {task}");
            Console.WriteLine("Добавлено!");
            sw.Stop();
            Tracer.TaskManagerTrace.TraceEvent(
                TraceEventType.Stop,
                1,
                $"Завершение AddTask. Время: {sw.ElapsedMilliseconds} мс"
                );
        }

        static void DeleteTask()
        {
            if (tasks.Count == 0)
            {
                Log.Warning("Попытка удаления при пустом списке");
                Console.WriteLine("Нет задач!");
                return;
            }

            ShowTasks();
            Console.Write("Какую удалить? ");

            if (!int.TryParse(Console.ReadLine(), out int index) || index < 1 || index > tasks.Count)
            {
                Log.Error("Ошибка ввода номера задачи для удаления");
                Console.WriteLine("Ошибка!");
                return;
            }

            string deletedTask = tasks[index-1];
            tasks.RemoveAt(index-1);
            completed.RemoveAt(index-1);
            Log.Information($"Удалена задача: {deletedTask}");
            Console.WriteLine("Удалено!");
        }

        static void ShowTasks()
        {
            if (tasks.Count == 0)
            {
                Log.Information("DEBUG: Отображение пустого списка задач");
                Console.WriteLine("Нет задач!");
                return;
            }

            Log.Information("DEBUG: Отображение списка задач");
            for (int i = 0; i < tasks.Count; i++)
            {
                Console.WriteLine($"{i+1}. {tasks[i]} {(completed[i] ? "[X]" : "[ ]")}");
            }
        }

        static void MarkCompleted()
        {
            if (tasks.Count == 0)
            {
                Log.Warning("Попытка отметить задачу при пустом списке");
                Console.WriteLine("Нет задач!");
                return;
            }

            ShowTasks();
            Console.Write("Какую отметить? ");

            if (!int.TryParse(Console.ReadLine(), out int index) || index < 1 || index > tasks.Count)
            {
                Log.Error("Ошибка ввода номера задачи для отметки");
                Console.WriteLine("Ошибка!");
                return;
            }

            completed[index-1] = true;
            Log.Information($"Задача отмечена как выполненная: {tasks[index-1]}");
            Console.WriteLine("Отмечено!");
        }
    }
}
