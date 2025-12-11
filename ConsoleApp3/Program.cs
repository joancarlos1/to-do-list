using System.Text.Json;

namespace ConsoleApp3
{
    internal class Program
    {
        
    }
}

class TaskItem {
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public bool Completed { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

class Program {
    static string DataFile = "tasks.json";
    static List<TaskItem> tasks = new List<TaskItem>();

    static void Main() {
        Load();
        while (true) {
            Console.Clear();
            ShowHeader();
            ShowMenu();
            var input = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(input)) continue;

            switch (input) {
                case "1": AddTask(); break;
                case "2": ListTasks(); Pause(); break;
                case "3": ToggleComplete(); break;
                case "4": RemoveTask(); break;
                case "5": ClearCompleted(); break;
                case "0": Save(); return;
                default: Console.WriteLine("Opção inválida."); Pause(); break;
            }
        }
    }

    static void ShowHeader() {
        Console.WriteLine("=== To-Do List (Console) ===");
        Console.WriteLine($"Tarefas: {tasks.Count}  |  Pendentes: {tasks.Count(t => !t.Completed)}  |  Completas: {tasks.Count(t => t.Completed)}");
        Console.WriteLine();
    }

    static void ShowMenu() {
        Console.WriteLine("1) Adicionar tarefa");
        Console.WriteLine("2) Listar tarefas");
        Console.WriteLine("3) Marcar/desmarcar como concluída");
        Console.WriteLine("4) Remover tarefa");
        Console.WriteLine("5) Remover tarefas concluídas");
        Console.WriteLine("0) Sair");
        Console.Write("Escolha: ");
    }

    static void AddTask() {
        Console.Write("Título: ");
        var title = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(title)) {
            Console.WriteLine("Título vazio — tarefa não adicionada.");
            Pause();
            return;
        }
        Console.Write("Descrição (opcional): ");
        var desc = Console.ReadLine()?.Trim();

        var t = new TaskItem { Title = title!, Description = string.IsNullOrEmpty(desc) ? null : desc };
        tasks.Add(t);
        Save();
        Console.WriteLine("Tarefa adicionada.");
        Pause();
    }

    static void ListTasks() {
        if (!tasks.Any()) {
            Console.WriteLine("Nenhuma tarefa encontrada.");
            return;
        }

        var ordered = tasks.OrderBy(t => t.Completed).ThenBy(t => t.CreatedAt);
        Console.WriteLine();
        foreach (var t in ordered) {
            var status = t.Completed ? "[X]" : "[ ]";
            Console.WriteLine($"{status} {t.Id.ToString().Substring(0, 8)} — {t.Title}");
            if (!string.IsNullOrEmpty(t.Description)) Console.WriteLine($"    {t.Description}");
            Console.WriteLine($"    Criado: {t.CreatedAt}");
        }
    }

    static void ToggleComplete() {
        ListTasks();
        Console.WriteLine();
        Console.Write("Digite os primeiros 8 caracteres do ID da tarefa para alternar (ou ENTER para cancelar): ");
        var partial = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(partial)) return;

        var task = tasks.FirstOrDefault(t => t.Id.ToString().StartsWith(partial, StringComparison.OrdinalIgnoreCase));
        if (task == null) {
            Console.WriteLine("Tarefa não encontrada.");
            Pause();
            return;
        }
        task.Completed = !task.Completed;
        Save();
        Console.WriteLine(task.Completed ? "Marcada como concluída." : "Marcada como pendente.");
        Pause();
    }

    static void RemoveTask() {
        ListTasks();
        Console.WriteLine();
        Console.Write("Digite os primeiros 8 caracteres do ID da tarefa a remover (ou ENTER para cancelar): ");
        var partial = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(partial)) return;

        var task = tasks.FirstOrDefault(t => t.Id.ToString().StartsWith(partial, StringComparison.OrdinalIgnoreCase));
        if (task == null) {
            Console.WriteLine("Tarefa não encontrada.");
            Pause();
            return;
        }
        tasks.Remove(task);
        Save();
        Console.WriteLine("Tarefa removida.");
        Pause();
    }

    static void ClearCompleted() {
        var removed = tasks.RemoveAll(t => t.Completed);
        Save();
        Console.WriteLine($"Removidas {removed} tarefas concluídas.");
        Pause();
    }

    static void Save() {
        try {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(tasks, options);
            File.WriteAllText(DataFile, json);
        }
        catch (Exception ex) {
            Console.WriteLine($"Erro ao salvar: {ex.Message} ");
        }
    }

    static void Load() {
        try {
            if (!File.Exists(DataFile)) return;
            var json = File.ReadAllText(DataFile);
            var loaded = JsonSerializer.Deserialize<List<TaskItem>>(json);
            if (loaded != null) tasks = loaded;
        }
        catch (Exception ex) {
            Console.WriteLine($"Erro ao carregar tarefas: {ex.Message}");
            Pause();
        }
    }

    static void Pause() {
        Console.WriteLine();
        Console.Write("Pressione ENTER para continuar...");
        Console.ReadLine();
        }
    }