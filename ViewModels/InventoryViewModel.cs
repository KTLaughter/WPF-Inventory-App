using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows;
using InventoryAppV1.Messages;
using System.Collections.ObjectModel;
using static InventoryAppV1.ViewModels.MainWindowViewModel;

namespace InventoryAppV1.ViewModels
{
    public class InventoryViewModel : ViewModelBase
    {
        private int _id;

        public int Id
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    _id = value;
                    RaisePropertyChanged(nameof(Id));
                }
            }
        }

        private string _inventoryName;

        public string InventoryName
        {
            get { return _inventoryName; }
            set
            {
                if (_inventoryName != value)
                {
                    _inventoryName = value;
                    RaisePropertyChanged(nameof(InventoryName));
                }
            }
        }

        private RelayCommand _openInventoryCommand;

        public RelayCommand OpenInventoryCommand
        {
            get
            {
                return _openInventoryCommand ?? (_openInventoryCommand = new RelayCommand(() =>
                {
                    System.Diagnostics.Debug.WriteLine("OpenInventoryCommand executed!"); // This is just here for debugging
                    NavigationService.NavigateToInventoryDetails(this);
                    ClearSearch();
                    UpdateStatistics();//this ensures the statistics are up-to-date when the user first opens the inventory
                }));
            }
        }

        private RelayCommand _backToMainCommand;

        public RelayCommand BackToMainCommand
        {
            get
            {
                return _backToMainCommand ?? (_backToMainCommand = new RelayCommand(() =>
                {
                    NavigationService.NavigateBack();
                }));
            }
        }

        private RelayCommand _deleteInventoryCommand;

        public RelayCommand DeleteInventoryCommand
        {
            get
            {
                return _deleteInventoryCommand ?? (_deleteInventoryCommand = new RelayCommand(() =>
                {
                    var result = System.Windows.MessageBox.Show("Are you sure you want to delete this inventory?", "Confirmation", MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.Yes)
                    {
                        MessengerInstance.Send(new DeleteInventoryMessage(this));
                    }
                }));
            }
        }

        private ObservableCollection<ItemViewModel> _items;

        public ObservableCollection<ItemViewModel> Items
        {
            get { return _items ?? (_items = new ObservableCollection<ItemViewModel>()); }
            set { Set(ref _items, value); }
        }

        public RelayCommand AddItemCommand { get; }
        public RelayCommand<ItemViewModel> DeleteItemCommand { get; }

        public RelayCommand<ItemViewModel> EditItemCommand { get; }



        private int _uniqueIdentifierProperty;

        public int UniqueIdentifierProperty
        {
            get { return _uniqueIdentifierProperty; }
            set { Set(ref _uniqueIdentifierProperty, value); }
        }

        private InventoryService inventoryService;

        public InventoryViewModel()
        {
            //initialize all of the commands
            AddItemCommand = new RelayCommand(() => PromptUserForItemDetailsAndAddItem());
            DeleteItemCommand = new RelayCommand<ItemViewModel>(DeleteItem);
            inventoryService = new InventoryService();            
            EditItemCommand = new RelayCommand<ItemViewModel>(EditItem);
            UpdateStatistics();

            ClearSearchCommand = new RelayCommand(ClearSearch);

        }

        private void PromptUserForItemDetailsAndAddItem()
        {
            string itemName = Microsoft.VisualBasic.Interaction.InputBox("Enter Item Name:", "Add New Item");
            string costInput = Microsoft.VisualBasic.Interaction.InputBox("Enter Item Cost:", "Add New Item");
            string description = Microsoft.VisualBasic.Interaction.InputBox("Enter Item Description:", "Add New Item");
            string quantityInput = Microsoft.VisualBasic.Interaction.InputBox("Enter Item Quantity:", "Add New Item");

            // convert cost to correct type
            if (!string.IsNullOrWhiteSpace(costInput) && decimal.TryParse(costInput, out decimal cost))
            {
                if (!string.IsNullOrWhiteSpace(quantityInput) && int.TryParse(quantityInput, out int quantity))
                {
                    AddItem(itemName, cost, description, quantity);
                    UpdateStatistics();
                }
                else
                {
                    MessageBox.Show("Invalid quantity input. Please enter a valid number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Invalid cost input. Please enter a valid number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddItem(string itemName, decimal cost, string description, int quantity)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Item added!"); 

                // Create new item
                ItemViewModel newItem = new ItemViewModel
                {
                    ItemName = itemName,
                    Cost = cost,
                    Description = description,
                    Quantity = quantity,
                    InventoryId = Id,
                    ParentInventory = this
                };

                var item = inventoryService.SaveInventoryItem(newItem);
                newItem.Id = item.Id;

                // Add new item and update
                Items.Add(newItem);
                UpdateFilteredItems();
                UpdateStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving items: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void DeleteItem(ItemViewModel item)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this item?", "Confirmation", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                inventoryService.DeleteInventoryItem(item);
                Items.Remove(item);
                UpdateFilteredItems();
                UpdateStatistics();
            }

        }

            private int _totalItems;

            public int TotalItems
            {
                get { return _totalItems; }
                set { Set(ref _totalItems, value); }
            }

            private decimal _totalCost;

            public decimal TotalCost
            {
                get { return _totalCost; }
                set { Set(ref _totalCost, value); }
            }

        public void UpdateStatistics()
            {
                TotalItems = Items.Sum(item => item.Quantity);
                TotalCost = Items.Sum(item => item.Cost * item.Quantity);
            }

        public void EditItem(ItemViewModel item)
        {
            // Prompt user again
            string updatedName = Microsoft.VisualBasic.Interaction.InputBox("Enter Updated Item Name:", "Edit Item", item.ItemName);
            string updatedCostInput = Microsoft.VisualBasic.Interaction.InputBox("Enter Updated Item Cost:", "Edit Item", item.Cost.ToString());
            string updatedDescription = Microsoft.VisualBasic.Interaction.InputBox("Enter Updated Item Description:", "Edit Item", item.Description);
            string updatedQuantityInput = Microsoft.VisualBasic.Interaction.InputBox("Enter Updated Item Quantity:", "Edit Item", item.Quantity.ToString());

            if (!string.IsNullOrWhiteSpace(updatedName) && decimal.TryParse(updatedCostInput, out decimal updatedCost) && int.TryParse(updatedQuantityInput, out int updatedQuantity))
            {
                item.ItemName = updatedName;
                item.Cost = updatedCost;
                item.Description = updatedDescription;
                item.Quantity = updatedQuantity;

                inventoryService.UpdateInventoryItem(item);
               
                UpdateStatistics();
            }
            else
            {
                MessageBox.Show("Invalid input. Please enter valid details.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private string _searchText;

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    RaisePropertyChanged(nameof(SearchText));
                    UpdateFilteredItems();
                }
            }
        }

        private ObservableCollection<ItemViewModel> _filteredItems;

        public ObservableCollection<ItemViewModel> FilteredItems
        {
            get { return _filteredItems; }
            set
            {
                if (_filteredItems != value)
                {
                    _filteredItems = value;
                    RaisePropertyChanged(nameof(FilteredItems));
                }
            }
        }

        private void UpdateFilteredItems()
        {
            FilteredItems = new ObservableCollection<ItemViewModel>(
                Items.Where(item => item.ItemName.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
        }

        public RelayCommand ClearSearchCommand { get; }

        private void ClearSearch()
        {
            SearchText = string.Empty;
            UpdateFilteredItems();
            UpdateStatistics();
        }

    }
}