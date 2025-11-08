using System.Threading.Tasks;

namespace CoffeeShopApp.ViewModels;

public interface IRefreshable
{
    Task RefreshAsync();
}
