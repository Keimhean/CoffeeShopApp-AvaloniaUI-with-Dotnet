using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CoffeeShopApp.Data;
using CoffeeShopApp.Services;
using CoffeeShopApp.ViewModels;
using CoffeeShopApp.Views;

namespace CoffeeShopApp;

public partial class App : Application
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
    // DI (very simple) - SQL Server (Docker) connection
    var conn = "Server=localhost,14330;Database=CoffeeShopDB;User Id=sa;Password=YourPassword123!;Encrypt=true;TrustServerCertificate=true";
        var db = new Db(conn);
        var productRepo = new SqlProductRepository(db);
        var ordersRepo = new SqlOrderRepository(db);
        var itemsRepo = new SqlOrderItemRepository(db);
        var orderSvc = new OrderService(db, ordersRepo, itemsRepo);
        var reportSvc = new ReportService(db);

        var productsVM = new ProductListViewModel(productRepo);
        var ordersVM = new OrderViewModel(productRepo, orderSvc);
        var reportVM = new ReportViewModel(reportSvc);
        var mainVM = new MainViewModel(productsVM, ordersVM, reportVM);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = new MainWindow { DataContext = mainVM };

        base.OnFrameworkInitializationCompleted();
    }
}
