using InventoryAppV1.ViewModels;
using System.Windows.Controls;
namespace InventoryAppV1
{
    public partial class InventoryDetailsView : UserControl
    {
        public InventoryDetailsView()
        {
            InitializeComponent();
            DataContext = new InventoryViewModel();
        }

        private void ItemDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var viewModel = DataContext as InventoryViewModel;
            if (viewModel != null)
            {
                var selectedItem = (sender as ListBox).SelectedItem as ItemViewModel;
                if (selectedItem != null)
                {
                    selectedItem.ShowDescriptionCommand.Execute(null);
                }
            }
        }
    }
}

