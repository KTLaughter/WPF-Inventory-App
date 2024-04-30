using InventoryAppV2;

namespace InventoryAppV1
{
    public class Inventory
    {
        public int Id { get; set; }
        public string InventoryName { get; set; }
        public virtual ICollection<Item> Items { get; set; }
    }
}
