import os # Import the os module
import json # Import the json module

def addExpense():
    description = input("Enter the description of the expense: ") # Prompt user for expense description
    amount = float(input("Enter the amount of the expense: ")) # Prompt user for expense amount and convert it to float

    if amount <= 0: # Check if the amount is less than or equal to zero
        print("Error, amount must be greater than zero.") # Print error message
        return # Exit the function
    else:
        expense = {"description": description, "amount": amount} # Create a dictionary for the expense
        if os.path.exists("expenses.json"): # Check if the expenses.json file exists
            with open("expenses.json", "r") as file: # Open the file in read mode
                expenses = json.load(file) # Load the existing expenses from the file
        else:
            expenses = [] # Initialize an empty list if the file does not exist

        expenses.append(expense) # Append the new expense to the list

        with open("expenses.json", "w") as file: # Open the file in write mode
            json.dump(expenses, file, indent=4) # Write the updated expenses list to the file with indentation

        print("Expense added successfully!") # Print success message

addExpense()