using InventoryAppV1.ViewModels;

namespace InventoryAppV1.Messages
{
    public class DeleteInventoryMessage
    {
        public InventoryViewModel Inventory { get; }

        public DeleteInventoryMessage(InventoryViewModel inventory)
        {
            Inventory = inventory;
        }
    }
}
