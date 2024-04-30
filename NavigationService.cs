using InventoryAppV1.ViewModels;

namespace InventoryAppV1
{
    public static class NavigationService
    {
        public static event Action<InventoryViewModel> NavigateToInventoryDetailsRequested;

        public static void NavigateToInventoryDetails(InventoryViewModel inventory)
        {
            NavigateToInventoryDetailsRequested?.Invoke(inventory);
        }

        public static void NavigateBack()
        {
            NavigateBackRequested?.Invoke();
        }

        public static event Action NavigateBackRequested;
    }

}
