<div align="center">

# 💰 Expense Tracker

A fast, colorful command-line expense tracker built in C# — track spending,
set monthly budgets, and export your data, all from the terminal. 

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-12.0-239120?style=flat-square&logo=csharp)
![Platform](https://img.shields.io/badge/platform-Windows%20%7C%20macOS%20%7C%20Linux-lightgrey?style=flat-square)
![License](https://img.shields.io/badge/license-MIT-green?style=flat-square)

</div>

---

## ✨ Features

| | |
|---|---|
| ✅ | Add, update, and delete expenses |
| ✅ | View all expenses in a clean terminal table |
| ✅ | Overall spending summary, broken down by category |
| ✅ | Monthly summary for any month of the current year |
| ✅ | Filter expenses by category |
| ✅ | Set a monthly budget and get warned when you go over |
| ✅ | Export everything to a CSV file |
| ✅ | Data persists locally as plain JSON — no database needed |

<details>
<summary>📸 What it looks like</summary>

```
=======================================
       💰  EXPENSE TRACKER  💰
=======================================

What would you like to do?
 1. Add an expense
 2. Update an expense
 3. Delete an expense
 4. View all expenses
 5. View overall summary
 6. View summary for a specific month
 7. Filter expenses by category
 8. Set a monthly budget
 9. Export expenses to CSV
 0. Exit
> 4

ID   Date        Category       Description                 Amount
------------------------------------------------------------------
1    2026-09-01  Food           Groceries                     $84.20
2    2026-09-03  Transport      Metro card top-up              $25.00
3    2026-09-10  Entertainment  Concert tickets                $120.00
------------------------------------------------------------------
                                Total:                        $229.20
```

</details>

---

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download) — runs on Windows, macOS, and Linux

Check you have it:
```bash
dotnet --version
```

### Installation & Run

```bash
# 1. Clone the repo
git clone https://github.com/your-username/expense-tracker.git
cd expense-tracker/ExpenseTracker

# 2. Run it
dotnet run
```

That's it — no external dependencies, no database setup. On first run, the app
creates `expenses.json` and `budgets.json` next to the program to store your data.

> 💡 **Tip:** Run `dotnet build` first if you just want to compile without launching the app.

---

## 📂 Project Structure

```
ExpenseTracker/
├── ExpenseTracker.csproj     # Project file — tells .NET how to build the app
├── Program.cs                # Menu loop, user input, and table/console formatting
├── Models/
│   └── Expense.cs            # The Expense data model (Id, Date, Description, Amount, Category)
└── Services/
    └── ExpenseService.cs     # Core logic: CRUD, summaries, budgets, CSV export, JSON persistence
```

This follows a simple **Model / Service / Program** separation:

- **Model** — defines the *shape* of the data
- **Service** — owns all the *logic* (no console code allowed in here — makes it easy to test)
- **Program** — handles *user interaction only*, delegating everything else to the service

---

## 🕹️ Usage

All features are driven from the in-app menu — just run `dotnet run` and follow the prompts.

| Action | Menu Option |
|---|---|
| Add an expense | `1` |
| Update an expense | `2` |
| Delete an expense | `3` |
| View all expenses | `4` |
| Overall summary | `5` |
| Monthly summary | `6` |
| Filter by category | `7` |
| Set a monthly budget | `8` |
| Export to CSV | `9` |
| Exit | `0` |

Your data lives in two local JSON files, so you can inspect, back up, or version-control them:

- `expenses.json` — every expense you've logged
- `budgets.json` — your monthly budget targets

---

## 🧠 Core Concepts Demonstrated

This project doubles as a small learning reference for C# fundamentals:

- Classes & properties (`Expense`)
- Generic collections — `List<T>`, `Dictionary<TKey,TValue>`
- LINQ (`.Where`, `.Sum`, `.GroupBy`, `.OrderBy`)
- Lambda expressions
- Nullable types (`int?`, `string?`) for safe optional input
- String interpolation & formatting (`$"{amount:C}"`)
- JSON serialization with `System.Text.Json`
- Local functions & top-level statements

Every non-trivial line in the source is commented with a short `// CONCEPT:` explanation.

---

## 🛣️ Roadmap / Ideas

- [ ] Sort expenses by amount or date
- [ ] Multi-year support (monthly summaries currently assume the current year)
- [ ] Unit tests for `ExpenseService`
- [ ] Recurring expenses
- [ ] Import from CSV

Contributions and suggestions are welcome — feel free to open an issue or PR!

---

## 📄 License

This project is licensed under the [MIT License](LICENSE).

<div align="center">

Made with ☕ and C#

</div>
