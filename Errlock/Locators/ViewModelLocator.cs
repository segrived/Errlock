using Errlock.ViewModels;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace Errlock.Locators
{
    public class ViewModelLocator
    {
        public MainWindowViewModel MainWindowVM
        {
            get { return ServiceLocator.Current.GetInstance<MainWindowViewModel>(); }
        }

        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<MainWindowViewModel>();
        }
    }
}