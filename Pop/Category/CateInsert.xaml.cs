using CashY.Pop.Category.ViewModel;
using CommunityToolkit.Maui.Views;
namespace CashY.Pop.Category;


public partial class CateInsert : Popup
{
	public CateInsert(CateInsertViewModel viewModel)
	{
		viewModel.cateInsert = this;
        this.BindingContext = viewModel;
		InitializeComponent();
	}
}