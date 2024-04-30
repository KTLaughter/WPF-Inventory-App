using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight;
using InventoryAppV1.Messages;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using InventoryAppV2;
using Microsoft.EntityFrameworkCore;

namespace InventoryAppV1.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private Frame _mainFrame;

        public Frame MainFrame
        {
            get { return _mainFrame; }
            set { Set(ref _mainFrame, value); }
        }

        public MainWindowViewModel()
        {
            MainFrame = new Frame();
            NavigationService.NavigateToInventoryDetailsRequested += NavigateToInventoryDetailsHandler;
            NavigationService.NavigateBackRequested += NavigateBack;

            // Create an instance of  the InventoryService
            _inventoryService = new InventoryService();

            // Load up the inventories from the database when the program opens
            Inventories = _inventoryService.LoadInventories();

            // Register to use the DeleteInventoryMessage
            Messenger.Default.Register<DeleteInventoryMessage>(this, DeleteInventoryHandler);
        }

        private void NavigateToInventoryDetailsHandler(InventoryViewModel inventory)
        {
            var inventoryDetailsView = new InventoryDetailsView();
            inventoryDetailsView.DataContext = inventory;
            MainFrame.Content = inventoryDetailsView;
        }

        private ObservableCollection<InventoryViewModel> _inventories;

        public ObservableCollection<InventoryViewModel> Inventories
        {
            get { return _inventories ?? (_inventories = new ObservableCollection<InventoryViewModel>()); }
            set { Set(ref _inventories, value); }
        }

        private ICommand _createInventoryCommand;
        private InventoryService _inventoryService;

        public ICommand CreateInventoryCommand
        {
            get
            {
                return _createInventoryCommand ?? (_createInventoryCommand = new RelayCommand(() =>
                {
                    // Prompt the user for an inventory name
                    string inventoryName = Microsoft.VisualBasic.Interaction.InputBox("Enter Inventory Name:", "Create New Inventory");

                    if (!string.IsNullOrWhiteSpace(inventoryName))
                    {
                        InventoryViewModel newInventoryViewModel = new InventoryViewModel();
                        newInventoryViewModel.InventoryName = inventoryName;
                        _inventoryService.SaveInventory(newInventoryViewModel);
                        Inventories.Add(newInventoryViewModel);
                    }
                }));
            }
        }


        public void NavigateBack()
        {
            MainFrame.Content = null;
        }

        // Defining InventoryService as a nested class
        public class InventoryService
        {
            private ObservableCollection<InventoryViewModel> _inventories;

            public ObservableCollection<InventoryViewModel> Inventories
            {
                get { return _inventories ?? (_inventories = new ObservableCollection<InventoryViewModel>()); }
                set { _inventories = value; }
            }

            public ObservableCollection<InventoryViewModel> LoadInventories()
            {
                using (var dbContext = new InventoryDbContext())
                {
                    var inventoryEntities = dbContext.Inventories.Include(x => x.Items).ToList();
                    return new ObservableCollection<InventoryViewModel>(inventoryEntities.Select(ConvertToViewModel));
                }
            }

            public void SaveInventory(InventoryViewModel inventoryViewModel)
            {
                using (var dbContext = new InventoryDbContext())
                {
                    var inventoryEntity = ConvertToEntityFrom(inventoryViewModel);
                    dbContext.Inventories.Add(inventoryEntity);
                    dbContext.SaveChanges();
                    inventoryViewModel.Id = inventoryEntity.Id;
                }
            }

            public Item SaveInventoryItem(ItemViewModel itemViewModel)
            {
                var item = ConvertToEntityFrom(itemViewModel);
                using (var dbContext = new InventoryDbContext())
                {
                    dbContext.Items.Add(item);
                    dbContext.SaveChanges();
                    return item;
                }
            }

            public ItemViewModel UpdateInventoryItem(ItemViewModel updatedItem)
            {
                using (var dbContext = new InventoryDbContext())
                {
                    // Find the existing item in the database
                    var existingItem = dbContext.Items.FirstOrDefault(i => i.Id == updatedItem.Id);

                    if (existingItem != null)
                    {
                        // Edit the existing item with the user's new input
                        existingItem.ItemName = updatedItem.ItemName;
                        existingItem.Cost = updatedItem.Cost;
                        existingItem.Description = updatedItem.Description;
                        existingItem.Quantity = updatedItem.Quantity;

                        // Save to the database
                        dbContext.SaveChanges();                        
                        return ConvertToItemViewModel(existingItem);
                    }
                }

                // If no item found or edited return null
                return null;
            }

        private InventoryViewModel ConvertToViewModel(Inventory inventoryEntity)
            {
                return new InventoryViewModel
                {
                    Id = inventoryEntity.Id,
                    InventoryName = inventoryEntity.InventoryName,
                    Items = inventoryEntity.Items != null ? new ObservableCollection<ItemViewModel>(inventoryEntity.Items.Select(ConvertToItemViewModel)) : new ObservableCollection<ItemViewModel>()
                };
            }

            private ItemViewModel ConvertToItemViewModel(Item itemEntity)
            {
                return new ItemViewModel
                {
                    Id = itemEntity.Id,
                    Cost = itemEntity.Cost,
                    ItemName = itemEntity.ItemName,
                    Description = itemEntity.Description,
                    Quantity = itemEntity.Quantity,
                };
            }


            private Inventory ConvertToEntityFrom(InventoryViewModel inventoryViewModel)
            {
                return new Inventory
                {
                    InventoryName = inventoryViewModel.InventoryName                   
                };
            }


            private Item ConvertToEntityFrom(ItemViewModel itemViewModel)
            {
                return new Item
                {
                    ItemName = itemViewModel.ItemName,
                    Cost = itemViewModel.Cost,
                    Description = itemViewModel.Description,
                    Quantity = itemViewModel.Quantity,
                    InventoryId = itemViewModel.InventoryId
                };
            }

            public void DeleteInventory(InventoryViewModel inventoryViewModel)
            {
                using (var dbContext = new InventoryDbContext())
                {
                    var inventory = dbContext.Inventories.Find(inventoryViewModel.Id);
                    if (inventory != null)
                    {
                        dbContext.Inventories.Remove(inventory);
                        dbContext.SaveChanges();
                    }
                }
            }

            public void DeleteInventoryItem(ItemViewModel itemViewModel)
            {
                using (var dbContext = new InventoryDbContext())
                {
                    var item = dbContext.Items.Find(itemViewModel.Id);
                    if (item != null)
                    {
                        dbContext.Items.Remove(item);
                        dbContext.SaveChanges();
                    }
                }
            }
        }

        private void DeleteInventoryHandler(DeleteInventoryMessage message)
        {          
            var result = System.Windows.MessageBox.Show($"Are you sure you want to delete {message.Inventory.InventoryName}?", "Confirmation", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                var inventoryToRemove = Inventories.FirstOrDefault(i => i == message.Inventory);

                if (inventoryToRemove != null)
                {
                    Inventories.Remove(inventoryToRemove);
                    _inventoryService.DeleteInventory(inventoryToRemove);

                }
            }
        }
    }
}