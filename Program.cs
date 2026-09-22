using ExpenseTracker.Models;
using ExpenseTracker.Services;

// -----------------------------------------------------------------------
// CONCEPT: This is "top-level statements" -- a modern C# feature that lets
// a console app run code directly in this file without wrapping it in a
// class Program { static void Main(string[] args) { ... } } boilerplate.
// Under the hood, the compiler still generates that Main method for you.
// -----------------------------------------------------------------------

var service = new ExpenseService();

Console.OutputEncoding = System.Text.Encoding.UTF8; // so box-drawing/emoji characters render correctly
PrintBanner();

bool running = true;
while (running) // CONCEPT: a "while" loop keeps showing the menu until the user chooses to quit
{
    PrintMenu();
    string? choice = Console.ReadLine();

    // CONCEPT: "switch" picks a branch based on a value, similar to a
    // chain of if/else if, but often clearer to read for many options.
    switch (choice)
    {
        case "1":
            AddExpenseFlow();
            break;
        case "2":
            UpdateExpenseFlow();
            break;
        case "3":
            DeleteExpenseFlow();
            break;
        case "4":
            ViewAllExpensesFlow();
            break;
        case "5":
            ViewSummaryFlow();
            break;
        case "6":
            ViewMonthlySummaryFlow();
            break;
        case "7":
            FilterByCategoryFlow();
            break;
        case "8":
            SetBudgetFlow();
            break;
        case "9":
            ExportCsvFlow();
            break;
        case "0":
            running = false;
            Console.WriteLine("\nGoodbye! Your expenses are saved in expenses.json.\n");
            break;
        default:
            PrintError("That's not a valid option. Please choose a number from the menu.");
            break;
    }
}

// =======================================================================
// Below this point are "local functions" -- helper functions defined
// inside the same file/scope, used to keep the main loop above short
// and readable. Each one handles one menu action.
// =======================================================================

void PrintBanner()
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("=======================================");
    Console.WriteLine("       💰  EXPENSE TRACKER  💰");
    Console.WriteLine("=======================================");
    Console.ResetColor();
}

void PrintMenu()
{
    Console.WriteLine();
    Console.WriteLine("What would you like to do?");
    Console.WriteLine(" 1. Add an expense");
    Console.WriteLine(" 2. Update an expense");
    Console.WriteLine(" 3. Delete an expense");
    Console.WriteLine(" 4. View all expenses");
    Console.WriteLine(" 5. View overall summary");
    Console.WriteLine(" 6. View summary for a specific month");
    Console.WriteLine(" 7. Filter expenses by category");
    Console.WriteLine(" 8. Set a monthly budget");
    Console.WriteLine(" 9. Export expenses to CSV");
    Console.WriteLine(" 0. Exit");
    Console.Write("> ");
}

void AddExpenseFlow()
{
    Console.Write("\nDescription: ");
    string description = Console.ReadLine() ?? "";

    decimal? amount = ReadDecimal("Amount: ");
    if (amount == null) return; // ReadDecimal already printed an error

    Console.Write("Category (press Enter for 'General'): ");
    string category = Console.ReadLine() ?? "";

    Expense expense = service.AddExpense(description, amount.Value, category);

    PrintSuccess($"Added expense #{expense.Id}: \"{expense.Description}\" - {expense.Amount:C} ({expense.Category})");

    CheckBudgetWarning(expense.Date.Month, expense.Date.Year);
}

void UpdateExpenseFlow()
{
    int? id = ReadInt("\nEnter the ID of the expense to update: ");
    if (id == null) return;

    Console.Write("New description (press Enter to keep current): ");
    string description = Console.ReadLine() ?? "";

    Console.Write("New amount (press Enter to keep current): ");
    string amountInput = Console.ReadLine() ?? "";
    decimal? amount = null;
    if (!string.IsNullOrWhiteSpace(amountInput))
    {
        if (!decimal.TryParse(amountInput, out decimal parsedAmount))
        {
            PrintError("Invalid amount. Update cancelled.");
            return;
        }
        amount = parsedAmount;
    }

    Console.Write("New category (press Enter to keep current): ");
    string category = Console.ReadLine() ?? "";

    bool updated = service.UpdateExpense(id.Value, description, amount, category);
    if (updated)
    {
        PrintSuccess($"Expense #{id} updated.");
    }
    else
    {
        PrintError($"No expense found with ID {id}.");
    }
}

void DeleteExpenseFlow()
{
    int? id = ReadInt("\nEnter the ID of the expense to delete: ");
    if (id == null) return;

    bool deleted = service.DeleteExpense(id.Value);
    if (deleted)
    {
        PrintSuccess($"Expense #{id} deleted.");
    }
    else
    {
        PrintError($"No expense found with ID {id}.");
    }
}

void ViewAllExpensesFlow()
{
    List<Expense> expenses = service.GetAllExpenses();
    Console.WriteLine();
    PrintExpensesTable(expenses);
}

void ViewSummaryFlow()
{
    decimal total = service.GetTotalSpending();
    Dictionary<string, decimal> byCategory = service.GetSpendingByCategory();

    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("=== Overall Summary ===");
    Console.ResetColor();
    Console.WriteLine($"Total spent: {total:C}");

    if (byCategory.Count > 0)
    {
        Console.WriteLine("\nBy category:");
        foreach (var pair in byCategory.OrderByDescending(p => p.Value))
        {
            Console.WriteLine($"  {pair.Key,-15} {pair.Value,10:C}");
        }
    }
}

void ViewMonthlySummaryFlow()
{
    int? month = ReadInt("\nEnter the month number (1-12): ");
    if (month == null || month < 1 || month > 12)
    {
        PrintError("Please enter a valid month between 1 and 12.");
        return;
    }

    int year = DateTime.Now.Year; // requirement says "of current year"
    decimal total = service.GetMonthlySpending(month.Value, year);
    List<Expense> monthExpenses = service.GetExpensesForMonth(month.Value, year);
    string monthName = new DateTime(year, month.Value, 1).ToString("MMMM");

    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine($"=== Summary for {monthName} {year} ===");
    Console.ResetColor();
    Console.WriteLine($"Total spent: {total:C}");

    PrintExpensesTable(monthExpenses);

    decimal? budget = service.GetBudget(month.Value, year);
    if (budget.HasValue)
    {
        Console.WriteLine($"Budget: {budget:C}");
        if (total > budget.Value)
        {
            PrintError($"⚠ You are {total - budget.Value:C} over budget for {monthName}!");
        }
        else
        {
            PrintSuccess($"You are {budget.Value - total:C} under budget for {monthName}.");
        }
    }
}

void FilterByCategoryFlow()
{
    Console.Write("\nEnter category to filter by: ");
    string category = Console.ReadLine() ?? "";
    List<Expense> filtered = service.GetAllExpenses(category);
    Console.WriteLine();
    PrintExpensesTable(filtered);
}

void SetBudgetFlow()
{
    int? month = ReadInt("\nEnter the month number (1-12): ");
    if (month == null || month < 1 || month > 12)
    {
        PrintError("Please enter a valid month between 1 and 12.");
        return;
    }

    decimal? amount = ReadDecimal("Monthly budget amount: ");
    if (amount == null) return;

    int year = DateTime.Now.Year;
    service.SetBudget(month.Value, year, amount.Value);
    string monthName = new DateTime(year, month.Value, 1).ToString("MMMM");
    PrintSuccess($"Budget for {monthName} {year} set to {amount:C}.");
}

void ExportCsvFlow()
{
    Console.Write("\nFile name for export (e.g. expenses.csv): ");
    string fileName = Console.ReadLine() ?? "";
    if (string.IsNullOrWhiteSpace(fileName))
    {
        fileName = "expenses.csv";
    }

    service.ExportToCsv(fileName);
    PrintSuccess($"Expenses exported to {Path.GetFullPath(fileName)}");
}

void CheckBudgetWarning(int month, int year)
{
    decimal? budget = service.GetBudget(month, year);
    if (budget == null) return;

    decimal spent = service.GetMonthlySpending(month, year);
    if (spent > budget.Value)
    {
        string monthName = new DateTime(year, month, 1).ToString("MMMM");
        PrintError($"⚠ Warning: you've exceeded your {monthName} budget! Spent {spent:C} of {budget:C}.");
    }
}

// ----------------------------- Formatting helpers -----------------------------

void PrintExpensesTable(List<Expense> expenses)
{
    if (expenses.Count == 0)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("No expenses to show.");
        Console.ResetColor();
        return;
    }

    // The "-5" etc. inside {value,-5} sets a minimum column width and
    // left-aligns (negative) or right-aligns (positive) the text --
    // this is how we get neatly lined-up table columns without a library.
    string header = $"{"ID",-5}{"Date",-12}{"Category",-15}{"Description",-28}{"Amount",12}";

    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine(header);
    Console.WriteLine(new string('-', header.Length));
    Console.ResetColor();

    foreach (Expense e in expenses)
    {
        string row = $"{e.Id,-5}{e.Date.ToString("yyyy-MM-dd"),-12}{Truncate(e.Category, 14),-15}{Truncate(e.Description, 27),-28}{e.Amount,12:C}";
        Console.WriteLine(row);
    }

    Console.WriteLine(new string('-', header.Length));
    decimal total = expenses.Sum(e => e.Amount);
    Console.WriteLine($"{"", -32}{"Total:",12}{total,12:C}");
}

string Truncate(string value, int maxLength)
{
    if (string.IsNullOrEmpty(value) || value.Length <= maxLength) return value;
    return value[..(maxLength - 3)] + "...";
}

void PrintSuccess(string message)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine(message);
    Console.ResetColor();
}

void PrintError(string message)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(message);
    Console.ResetColor();
}

// -------------------------- Safe input reading helpers --------------------------
// CONCEPT: Console.ReadLine() always returns a string (or null), so we need
// to validate and convert ("parse") it ourselves before using it as a
// number. These helpers wrap that pattern so we don't repeat it everywhere.

decimal? ReadDecimal(string prompt)
{
    Console.Write(prompt);
    string input = Console.ReadLine() ?? "";
    if (decimal.TryParse(input, out decimal value) && value >= 0)
    {
        return value;
    }

    PrintError("Please enter a valid, non-negative number.");
    return null;
}

int? ReadInt(string prompt)
{
    Console.Write(prompt);
    string input = Console.ReadLine() ?? "";
    if (int.TryParse(input, out int value))
    {
        return value;
    }

    PrintError("Please enter a valid whole number.");
    return null;
}
