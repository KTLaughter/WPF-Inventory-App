This is my Inventory app that I made for my capstone project at Limestone University. 
This app was made using C# with the following framworks: WPF(Windows Presentation Foundation), Entity Framework Core, and MvvMlight, alognside SQLite to handle the DB.

The purpose of the app is to allow users to keep track of their collections of items within their own virtual inventory.
I wanted to make a program that serves a real-world purpose, and I think this app could be of use to someone, especially if they are a collector who wants to keep track of what they have.

The main menu allows you to create a new inventory, and select and delete pre-exisitng inventories you have already made. 
Once inside an Inventory, you are able to create, edit, or delete an item(s). 
When creating an item, the user will be required to provide the item's name, cost, description, and quantity.
Once an item has been added to its inventory, it is written to the inventories.db file, meaning the items and inventories will be persistent even after the program is closed.
Another feature is the search bar found at the bottom of the inventory screen. Search for an item and it will display it (as long as the item is in the inventory). 
Lastly, each inventory has a total cost and total item number count displayed at the top left to give the user more information about their inventory.
