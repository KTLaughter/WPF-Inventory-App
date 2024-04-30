using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using InventoryAppV1.ViewModels;
using System.Windows;
using static InventoryAppV1.ViewModels.MainWindowViewModel;

namespace InventoryAppV1
{
    public class ItemViewModel : ViewModelBase
    {
        private int _id;
        private string _itemName;
        private decimal _cost;
        private string _description;
        private int _quantity;

        public int Id
        {
            get { return _id; }
            set { Set(ref _id, value); }
        }

        public string ItemName
        {
            get { return _itemName; }
            set { Set(ref _itemName, value); }
        }

        public decimal Cost
        {
            get { return _cost; }
            set { Set(ref _cost, value); }
        }

        public string Description
        {
            get { return _description; }
            set { Set(ref _description, value); }
        }


        public int Quantity
        {
            get { return _quantity; }
            set { Set(ref _quantity, value);}
        }

        //to show the item description in a popup window
        public RelayCommand ShowDescriptionCommand { get; }

        private InventoryService _inventoryService;

        public ItemViewModel()
        {
            ShowDescriptionCommand = new RelayCommand(ShowDescription);

            _inventoryService = new InventoryService();
        }

        private void ShowDescription()
        {
            String title = ItemName + "'s description";

            MessageBox.Show(Description, title , MessageBoxButton.OK);
        }

        public int InventoryId { get; set; }

        public virtual InventoryViewModel ParentInventory { get; set; }
    }
}
