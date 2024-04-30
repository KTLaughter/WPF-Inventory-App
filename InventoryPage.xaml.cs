using System.Windows.Controls;

namespace InventoryAppV1
{
    public partial class InventoryPage : Page
    {
        public string InventoryName { get; set; }

        public InventoryPage()
        {
            InitializeComponent();

            DataContext = this;
        }
    }
}
