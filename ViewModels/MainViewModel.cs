using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CoffeeShopApp.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty] private ViewModelBase _content;
    [ObservableProperty] private bool _isProductsSelected;
    [ObservableProperty] private bool _isOrdersSelected;
    [ObservableProperty] private bool _isReportSelected;
    public IRelayCommand ShowProductsCmd { get; }
    public IRelayCommand ShowOrdersCmd { get; }
    public IRelayCommand ShowReportCmd { get; }
    public IAsyncRelayCommand RefreshCmd { get; }

    public MainViewModel(ViewModelBase products, ViewModelBase orders, ViewModelBase report)
    {
        _content = products;
        IsProductsSelected = true;
        ShowProductsCmd = new RelayCommand(() => Content = products);
        ShowOrdersCmd = new RelayCommand(() => { Content = orders; SetSelected("orders"); });
        ShowReportCmd = new RelayCommand(() => { Content = report; SetSelected("report"); });
        ShowProductsCmd = new RelayCommand(() => { Content = products; SetSelected("products"); });
        RefreshCmd = new AsyncRelayCommand(RefreshAsync);
    }

    private void SetSelected(string which)
    {
        IsProductsSelected = which == "products";
        IsOrdersSelected = which == "orders";
        IsReportSelected = which == "report";
    }

    private async Task RefreshAsync()
    {
        if (Content is IRefreshable r)
        {
            await r.RefreshAsync();
        }
    }
}
