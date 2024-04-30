using System.Configuration;
using System.Data;
using System.Windows;
using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using InventoryAppV1.ViewModels;


namespace InventoryAppV1
{
    public partial class App
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            using (var dbContext = new InventoryDbContext())
            {
                dbContext.Database.EnsureCreated();
            }

            base.OnStartup(e);
        }

        static App()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            // Register the ViewModels
            SimpleIoc.Default.Register<MainWindowViewModel>();
        }

        public static MainWindowViewModel MainViewModel => ServiceLocator.Current.GetInstance<MainWindowViewModel>();
    }
}