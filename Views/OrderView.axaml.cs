using Avalonia.Controls;
using Avalonia.VisualTree;
using CoffeeShopApp.ViewModels;

namespace CoffeeShopApp.Views;

public partial class OrderView : UserControl
{
	public OrderView()
	{
		InitializeComponent();
		// When the view is attached to the visual tree, load product choices
		this.AttachedToVisualTree += (_, _) =>
		{
			if (DataContext is OrderViewModel vm)
			{
				_ = vm.LoadChoicesCmd.ExecuteAsync(null);
			}
		};
	}
    
}
