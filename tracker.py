import os # Import the os module
import json # Import the json module
import datetime # Import the datetime module
import random # Import the random module


EXPENSES_FILE = "expenses.json" # Define a constant for the expenses file name


def addExpense():
    description = input("Enter the description of the expense: ") # Prompt user for expense description
    if not description: # Check if the description is empty
        print("Error, description cannot be empty.") # Print error message
        return # Exit the function

    try:
        amount = float(input("Enter the amount of the expense: ")) # Prompt user for expense amount and convert it to float
    except ValueError:
        print("Error, invalid amount entered.") # Print error message
        return # Exit the function

    if amount <= 0: # Check if the amount is less than or equal to zero
        print("Error, amount must be greater than zero.") # Print error message
        return # Exit the function

    if os.path.exists(EXPENSES_FILE): # Check if the expenses file exists
        with open(EXPENSES_FILE, 'r') as f: # Open the expenses file in read mode
            try:
                expensesList = json.load(f) # Load the expenses from the file
            except json.JSONDecodeError:
                expensesList = [] # If the file is empty or invalid, initialize an empty list
    else:
        expensesList = [] # If the file does not exist, initialize an empty list    

    expensesList.append({
        'id': random.randint(1000, 9999),  # Assign a random 4-digit ID
        'description': description, # Add the expense description to the list
        'amount': amount, # Add the expense amount to the list
        'date': datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S") # Add the current date and time to the list
    })

    with open(EXPENSES_FILE, 'w') as f: # Open the expenses file in write mode
        json.dump(expensesList, f, indent=2) # Write the updated expenses list to the file

    print(f'Expense "{description}" of amount ${amount:.2f} added successfully.') # Print success message

addExpense()