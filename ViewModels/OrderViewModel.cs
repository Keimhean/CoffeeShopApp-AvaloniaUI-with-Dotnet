using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CoffeeShopApp.Data;
using CoffeeShopApp.Models;
using CoffeeShopApp.Services;

namespace CoffeeShopApp.ViewModels;

public partial class OrderViewModel : ViewModelBase
{
    private readonly IRepository<Product> _products;
    private readonly OrderService _service;

    public ObservableCollection<Product> ProductChoices { get; } = new();
    public ObservableCollection<OrderItem> Items { get; } = new();

    [ObservableProperty] private decimal _tendered;
    [ObservableProperty] private string? _error;
    [ObservableProperty] private string? _completedSummary;

    [ObservableProperty] private Product? _selectedProduct;
    [ObservableProperty] private int _qty = 1;
    [ObservableProperty] private string? _cashier;
    public decimal Total => Items.Sum(i => i.Subtotal);
    public decimal Change => Tendered - Total;

    public IAsyncRelayCommand LoadChoicesCmd { get; }
    public IRelayCommand AddToOrderCmd { get; }
    public IAsyncRelayCommand CompleteCmd { get; }
    public IRelayCommand IncrementQtyCmd { get; }
    public IRelayCommand DecrementQtyCmd { get; }
    public ObservableCollection<Order> RecentOrders { get; } = new();
    public IAsyncRelayCommand LoadRecentCmd { get; }

    public OrderViewModel(IRepository<Product> products, OrderService service)
    {
        _products = products; _service = service;
        LoadChoicesCmd = new AsyncRelayCommand(LoadChoicesAsync);
        AddToOrderCmd = new RelayCommand(AddToOrder, () => SelectedProduct is not null && Qty > 0);
        CompleteCmd = new AsyncRelayCommand(CompleteAsync);
        IncrementQtyCmd = new RelayCommand(() => Qty++);
        DecrementQtyCmd = new RelayCommand(() => { if (Qty>1) Qty--; });
        LoadRecentCmd = new AsyncRelayCommand(LoadRecentAsync);

        // initial loads
        _ = LoadChoicesAsync();
        _ = LoadRecentAsync();

        // keep Total and command state updated when Items change
        Items.CollectionChanged += (s, e) => {
            OnPropertyChanged(nameof(Total));
            // notify commands that may depend on items/total
            AddToOrderCmd.NotifyCanExecuteChanged();
            if (CompleteCmd is IAsyncRelayCommand asyncCmd)
                asyncCmd.NotifyCanExecuteChanged();
            else if (CompleteCmd is IRelayCommand cmd)
                cmd.NotifyCanExecuteChanged();
        };
    }

    public Task RefreshAsync()
    {
        // refresh product choices and notify UI
        return LoadChoicesAsync();
    }

    partial void OnTenderedChanged(decimal value) => OnPropertyChanged(nameof(Change));

    partial void OnSelectedProductChanged(Product? value) => AddToOrderCmd.NotifyCanExecuteChanged();
    partial void OnQtyChanged(int value) => AddToOrderCmd.NotifyCanExecuteChanged();

    private async Task LoadRecentAsync()
    {
        RecentOrders.Clear();
        var list = await _service.GetRecentAsync(10);
        foreach (var o in list) RecentOrders.Add(o);
    }

    private async Task LoadChoicesAsync()
    {
        ProductChoices.Clear();
        foreach (var p in await _products.GetAllAsync()) ProductChoices.Add(p);
        if (ProductChoices.Any()) SelectedProduct = ProductChoices.First();
    }

    private void AddToOrder()
    {
        if (SelectedProduct is null) return;
        var exist = Items.FirstOrDefault(i => i.ProductId == SelectedProduct.Id);
        if (exist is null)
            Items.Add(new OrderItem { ProductId = SelectedProduct.Id, Qty = Qty, UnitPrice = SelectedProduct.Price, Product = SelectedProduct });
        else
            exist.Qty += Qty;
        OnPropertyChanged(nameof(Total));
    }

    private async Task CompleteAsync()
    {
        // cannot complete empty order
        if (Total <= 0)
        {
            Error = "No items in order";
            return;
        }

        // if user didn't input tendered (0), assume exact payment
        if (Tendered <= 0)
        {
            Tendered = Total;
        }

        // validate payment
        if (Tendered < Total)
        {
            Error = "Insufficient payment";
            return;
        }

        Error = null;

        var order = new Order { Cashier = Cashier };
        foreach (var it in Items)
            order.Items.Add(new OrderItem { ProductId = it.ProductId, Qty = it.Qty, UnitPrice = it.UnitPrice });
        var id = await _service.CompleteAsync(order);

        // build a simple receipt/summary
        CompletedSummary = $"Order: {order.OrderNumber}\n" +
            $"Items: {order.Items.Count}  Total: $ {order.Total:F2}\n" +
            $"Tendered: $ {Tendered:F2}  Change: $ {Change:F2}\n" +
            $"Order Id: {id}";

        Items.Clear();
        Qty = 1;
        Tendered = 0m;
        OnPropertyChanged(nameof(Total));
        OnPropertyChanged(nameof(Change));
    await LoadRecentAsync();
    }
}
