using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CoffeeShopApp.Services;
using CoffeeShopApp.Utils;
using CoffeeShopApp.ViewModels;

namespace CoffeeShopApp.ViewModels;

public partial class ReportViewModel : ViewModelBase, IRefreshable
{
    private readonly ReportService _svc;
    [ObservableProperty] private DateTime _from = DateTime.Today.AddDays(-7);
    [ObservableProperty] private DateTime _to = DateTime.Today;
    [ObservableProperty] private int _totalOrders;
    [ObservableProperty] private decimal _totalRevenue;

    public ObservableCollection<ReportService> Dummy { get; } = new(); // placeholder to keep simple
    public ObservableCollection<ReportRow> Rows { get; } = new();

    public IAsyncRelayCommand LoadCmd { get; }

    public ReportViewModel(ReportService svc)
    {
        _svc = svc;
        LoadCmd = new AsyncRelayCommand(LoadAsync);
    }

    public Task RefreshAsync() => LoadAsync();

    private async Task LoadAsync()
    {
        Rows.Clear();
        var list = await _svc.GetDailyAsync(From, To);
        foreach (var r in list)
        {
            var utc = r.Date;
            var local = TimeZoneHelper.ToCambodiaTime(utc);
            Rows.Add(new ReportRow
            {
                DateUtc = utc,
                DateLocal = local,
                DateLocalString = local.ToString("g"),
                Orders = r.Orders,
                Revenue = r.Revenue
            });
        }
        // compute totals
        TotalOrders = list.Sum(x => x.Orders);
        TotalRevenue = list.Sum(x => x.Revenue);
    }
}
