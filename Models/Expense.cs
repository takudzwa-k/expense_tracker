namespace ExpenseTracker.Models;

// ---------------------------------------------------------------------
// CONCEPT: A "class" is a blueprint for an object. Here, Expense is the
// blueprint for a single expense record. Every expense we create in the
// app (e.g. "Coffee, $4.50") will be an "instance" of this class.
// ---------------------------------------------------------------------
public class Expense
{
    // ---------------------------------------------------------------
    // CONCEPT: "Properties" (the { get; set; } parts) are like variables
    // that belong to the object. "get" lets you read the value, "set"
    // lets you change it. This is shorter than writing separate
    // GetId()/SetId() methods like older languages often require.
    // ---------------------------------------------------------------
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Category { get; set; } = "General";

    // "decimal" (not double/float) is the correct C# type for money,
    // because it avoids the tiny rounding errors that floating-point
    // types can introduce with currency values.
}
