using TreeView.ViewModels;

namespace TreeView
{
    public class VmLocator
    {
        public static MainViewModel MainViewModel => Ioc.Resolve<MainViewModel>();
    }
}
