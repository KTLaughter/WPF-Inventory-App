using InventoryAppV1;

namespace InventoryAppV2
{
    public class Item
    {
        public int Id { get; set; }
        public string ItemName { get; set; }
        public decimal Cost { get; set; }
        public string Description { get; set; }
        public int InventoryId { get; set; } // this the foreign key
        public int Quantity { get; set; }
        public virtual Inventory Inventory { get; set; } // Navigation property
    }
}
