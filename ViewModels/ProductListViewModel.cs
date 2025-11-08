using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CoffeeShopApp.Data;
using CoffeeShopApp.Models;

namespace CoffeeShopApp.ViewModels;

public partial class ProductListViewModel : ViewModelBase, IRefreshable
{
    private readonly IRepository<Product> _repo;

    public ObservableCollection<Product> Items { get; } = new();
    [ObservableProperty] private string _searchText = "";
    public IAsyncRelayCommand LoadCmd { get; }
    public IAsyncRelayCommand AddCmd { get; }
    public IAsyncRelayCommand RefreshCmd { get; }
    public IAsyncRelayCommand<Product> SaveCmd { get; }
    public IAsyncRelayCommand<Product> DeleteCmd { get; }

    public ProductListViewModel(IRepository<Product> repo)
    {
        _repo = repo;
        LoadCmd = new AsyncRelayCommand(LoadAsync);
        RefreshCmd = new AsyncRelayCommand(LoadAsync);
        AddCmd = new AsyncRelayCommand(AddAsync);
        SaveCmd = new AsyncRelayCommand<Product>(SaveAsync);
        DeleteCmd = new AsyncRelayCommand<Product>(DeleteAsync);
        // Auto-load products when the view model is created so the UI shows items immediately
        _ = LoadAsync();
    }

    public Task RefreshAsync() => LoadAsync();

    private async Task LoadAsync()
    {
        Items.Clear();
        var list = string.IsNullOrWhiteSpace(SearchText)
            ? await _repo.GetAllAsync()
            : await _repo.SearchAsync(SearchText);
        foreach (var p in list) Items.Add(p);
    }

    private async Task AddAsync()
    {
        var p = new Product { Name = "New", Category = "Coffee", Price = 1.00m, Stock = 0 };
        p.Id = await _repo.AddAsync(p);
        Items.Add(p);
    }

    private async Task SaveAsync(Product? p)
    {
        if (p is null) return;
        await _repo.UpdateAsync(p);
    }

    private async Task DeleteAsync(Product? p)
    {
        if (p is null) return;
        if (await _repo.DeleteAsync(p.Id)) Items.Remove(p);
    }
}
