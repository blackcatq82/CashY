using CommunityToolkit.Maui.Views;
namespace CashY.Pop;
public partial class Message : Popup
{
    public Message(MessageViewModel vm)
    {
        if (BindingContext != vm)
            BindingContext = vm;

        InitializeComponent();
    }

    // handler button close pop.
    public async void ClosePop(object sender, System.EventArgs e)
    {
        // Close all PopupPages in the PopupStack
        await CloseAsync();
    }
}