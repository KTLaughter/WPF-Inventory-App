using GalaSoft.MvvmLight.Messaging;
using System.Runtime.CompilerServices;

namespace InventoryAppV1
{
    public static class PropertyHelper
    {
        public static bool Set<T>(ref T field, T newValue = default(T), bool broadcast = false, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return false;
            }

            T oldValue = field;
            field = newValue;
            RaisePropertyChanged(propertyName, oldValue, field, broadcast);
            return true;
        }

        private static void RaisePropertyChanged<T>(string propertyName, T oldValue, T newValue, bool broadcast)
        {
            if (broadcast)
            {
                PropertyChangedMessage<T> message = new PropertyChangedMessage<T>(null, oldValue, newValue, propertyName);
                Messenger.Default.Send(message);
            }
        }
    }
}
