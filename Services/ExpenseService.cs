using System.Text.Json;
using ExpenseTracker.Models;

namespace ExpenseTracker.Services;

// -----------------------------------------------------------------------
// CONCEPT: This is a "service" class. It's a common pattern: keep all the
// logic for working with your data (loading, saving, adding, deleting,
// calculating totals...) in one place, separate from the code that talks
// to the user (that lives in Program.cs). This separation makes each
// piece easier to understand, test, and change independently.
// -----------------------------------------------------------------------
public class ExpenseService
{
    // "private readonly" fields: only this class can see them (private),
    // and once set in the constructor they can never be reassigned
    // (readonly). Good defaults unless you have a reason to relax them.
    private readonly string _dataFile;
    private readonly string _budgetFile;

    // List<Expense> is C#'s generic, resizable array type -- like a
    // Python list or a JS array, but strongly typed: this list can only
    // ever hold Expense objects.
    private List<Expense> _expenses;

    // Dictionary<TKey, TValue> is a hash map / associative array.
    // We use it to store one budget per month, keyed by "yyyy-MM",
    // e.g. "2026-09" -> 500.00m
    private Dictionary<string, decimal> _budgets;

    // ---------------------------------------------------------------
    // CONCEPT: A "constructor" runs once, when you create the object
    // with `new ExpenseService()`. It's the natural place to load our
    // saved data from disk so the app starts up with existing expenses.
    // ---------------------------------------------------------------
    public ExpenseService(string dataFile = "expenses.json", string budgetFile = "budgets.json")
    {
        _dataFile = dataFile;
        _budgetFile = budgetFile;
        _expenses = LoadExpenses();
        _budgets = LoadBudgets();
    }

    // ----------------------- Persistence (JSON) ----------------------
    // CONCEPT: We store data as JSON text files on disk so expenses
    // survive between runs of the program. System.Text.Json (built into
    // .NET, no extra install needed) converts our C# objects to/from
    // JSON automatically based on their public properties.

    private List<Expense> LoadExpenses()
    {
        if (!File.Exists(_dataFile))
        {
            return new List<Expense>();
        }

        string json = File.ReadAllText(_dataFile);
        // Deserialize turns JSON text back into real C# objects.
        // The "?? new List<Expense>()" is the null-coalescing operator:
        // if deserialization somehow returns null, use an empty list instead.
        return JsonSerializer.Deserialize<List<Expense>>(json) ?? new List<Expense>();
    }

    private void SaveExpenses()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(_expenses, options);
        File.WriteAllText(_dataFile, json);
    }

    private Dictionary<string, decimal> LoadBudgets()
    {
        if (!File.Exists(_budgetFile))
        {
            return new Dictionary<string, decimal>();
        }

        string json = File.ReadAllText(_budgetFile);
        return JsonSerializer.Deserialize<Dictionary<string, decimal>>(json) ?? new Dictionary<string, decimal>();
    }

    private void SaveBudgets()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(_budgets, options);
        File.WriteAllText(_budgetFile, json);
    }

    // ------------------------------- CRUD -----------------------------
    // CRUD = Create, Read, Update, Delete -- the four basic operations
    // almost every data-driven app needs.

    public Expense AddExpense(string description, decimal amount, string category)
    {
        // Pick the next ID: one higher than the current max, or 1 if the list is empty.
        // The ternary `condition ? a : b` is shorthand for a simple if/else that returns a value.
        int newId = _expenses.Count == 0 ? 1 : _expenses.Max(e => e.Id) + 1;

        var expense = new Expense
        {
            Id = newId,
            Date = DateTime.Now,
            Description = description,
            Amount = amount,
            Category = string.IsNullOrWhiteSpace(category) ? "General" : category
        };

        _expenses.Add(expense);
        SaveExpenses();
        return expense;
    }

    public bool UpdateExpense(int id, string? description, decimal? amount, string? category)
    {
        // FirstOrDefault: scan the list for the first item matching the condition,
        // or return null (default for a reference type) if nothing matches.
        // e => e.Id == id is a "lambda expression": a tiny inline function
        // meaning "given an expense e, check if its Id equals id".
        Expense? expense = _expenses.FirstOrDefault(e => e.Id == id);
        if (expense == null)
        {
            return false;
        }

        // Only overwrite fields the caller actually provided.
        // string? and decimal? mean "this can be null / not supplied".
        if (!string.IsNullOrWhiteSpace(description)) expense.Description = description;
        if (amount.HasValue) expense.Amount = amount.Value;
        if (!string.IsNullOrWhiteSpace(category)) expense.Category = category;

        SaveExpenses();
        return true;
    }

    public bool DeleteExpense(int id)
    {
        Expense? expense = _expenses.FirstOrDefault(e => e.Id == id);
        if (expense == null)
        {
            return false;
        }

        _expenses.Remove(expense);
        SaveExpenses();
        return true;
    }

    public List<Expense> GetAllExpenses(string? categoryFilter = null)
    {
        // LINQ ("Language Integrated Query") lets you filter/sort/transform
        // collections in a very readable way, similar to SQL.
        IEnumerable<Expense> query = _expenses;

        if (!string.IsNullOrWhiteSpace(categoryFilter))
        {
            query = query.Where(e => e.Category.Equals(categoryFilter, StringComparison.OrdinalIgnoreCase));
        }

        return query.OrderBy(e => e.Id).ToList();
    }

    // ----------------------------- Summaries ---------------------------

    public decimal GetTotalSpending() => _expenses.Sum(e => e.Amount);

    // "=>" here is an "expression-bodied member": shorthand for a method
    // whose body is a single expression, instead of writing
    // { return ...; } with curly braces.

    public decimal GetMonthlySpending(int month, int year) =>
        _expenses.Where(e => e.Date.Month == month && e.Date.Year == year)
                 .Sum(e => e.Amount);

    public Dictionary<string, decimal> GetSpendingByCategory() =>
        _expenses.GroupBy(e => e.Category)
                 .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount));

    public List<Expense> GetExpensesForMonth(int month, int year) =>
        _expenses.Where(e => e.Date.Month == month && e.Date.Year == year)
                 .OrderBy(e => e.Date)
                 .ToList();

    // ------------------------------- Budgets ----------------------------

    public void SetBudget(int month, int year, decimal amount)
    {
        // $"..." is a "string interpolation" -- embed variables directly
        // in a string instead of concatenating with +.
        // "D2" formats the month as 2 digits, e.g. 9 -> "09".
        string key = $"{year}-{month:D2}";
        _budgets[key] = amount;
        SaveBudgets();
    }

    public decimal? GetBudget(int month, int year)
    {
        string key = $"{year}-{month:D2}";
        // TryGetValue avoids throwing an exception if the key is missing;
        // it returns true/false and gives you the value via an "out" parameter.
        return _budgets.TryGetValue(key, out decimal value) ? value : null;
    }

    // ------------------------------ CSV Export ---------------------------

    public void ExportToCsv(string filePath)
    {
        // "using" ensures the file is properly closed/flushed even if
        // something goes wrong while writing -- it calls Dispose()
        // automatically when the block ends.
        using var writer = new StreamWriter(filePath);

        writer.WriteLine("Id,Date,Description,Category,Amount");

        foreach (Expense e in _expenses.OrderBy(x => x.Id))
        {
            // Basic CSV escaping: if the description contains a comma,
            // wrap it in quotes so it isn't misread as extra columns.
            string description = e.Description.Contains(',') ? $"\"{e.Description}\"" : e.Description;
            writer.WriteLine($"{e.Id},{e.Date:yyyy-MM-dd},{description},{e.Category},{e.Amount}");
        }
    }
}
