using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;

using CashY.Services;
using CashY.Views;
using CashY.ViewModels;
using CashY.Views.ViewsModel;
using CashY.Model.Items;
using CashY.Pop.Category;
using CashY.Pop.Category.ViewModel;
using CashY.Model.Items.Items;
using CashY.Pop.Items;
using CashY.Pop.Items.ViewModel;
using CashY.Model.Items.Logger;
using CashY.Model.Items.Payment;
namespace CashY;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseSkiaSharp()
			.UseMauiCommunityToolkit()
			.RegisterPopups()
			.RegisterServices()
			.RegisterViews()
			.RegisterViewModels()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				fonts.AddFont("fontello.ttf", "Icons");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}

    public static MauiAppBuilder RegisterServices(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<IMyHttpRequests, MyHttpRequests>();
        builder.Services.AddSingleton<ILoginService, LoginService>();
        builder.Services.AddSingleton<IConnection, Connection>();
        builder.Services.AddSingleton<ICategoryServices, CategoryServices>();
        builder.Services.AddSingleton<IItemsServices, ItemsServices>();
        builder.Services.AddSingleton<ILoggerServices, LoggerServices>();
        builder.Services.AddSingleton<IPaymentServices, PaymentServices>();
        builder.Services.AddSingleton<IIPermissionServices, IPermissionsServices>();
        builder.Services.AddSingleton<IPopupServices, PopupServices>();
        builder.Services.AddSingleton<ILoadData, LoadData>();
        builder.Services.AddSingleton<IDatabaseServices, DatabaseServices>();
        return builder;
    }

    public static MauiAppBuilder RegisterPopups(this MauiAppBuilder builder)
	{
        builder.Services.AddTransient<Pop.MessageViewModel>();
        builder.Services.AddTransient<Pop.Message>();
        builder.Services.AddTransient<Pop.BusyIndicator>();
        builder.Services.AddTransient<Category>();
        builder.Services.AddTransient<MyItem>();
        builder.Services.AddTransient<MyLogger>();
        builder.Services.AddTransient<MyPayment>();
        builder.Services.AddTransient<CateInsertViewModel>();
        builder.Services.AddTransient<CateInsert>();
        builder.Services.AddTransient<PopupInsertItemViewModel>();
        builder.Services.AddTransient<PopupInsertItem>();
        return builder;
    }

    public static MauiAppBuilder RegisterViews(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<CategoryPage>();
        builder.Services.AddSingleton<HistoryPage>();
        builder.Services.AddSingleton<HomePage>();
        builder.Services.AddSingleton<ItemsPage>();
        builder.Services.AddSingleton<PaymentPage>();
        builder.Services.AddSingleton<LoadPage>();
        builder.Services.AddSingleton<AppShell>();
        builder.Services.AddSingleton<Login>();
        return builder;
    }



    public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder builder)
	{
		builder.Services.AddSingleton<LoadPageViewModel>();
		builder.Services.AddSingleton<ShellViewModel>();
		builder.Services.AddSingleton<LoginViewModel>();
		builder.Services.AddSingleton<CategoryPageViewModel>();
		builder.Services.AddSingleton<HomePageViewModel>();
		builder.Services.AddSingleton<PaymentPageViewModel>();
		builder.Services.AddSingleton<HistoryPageViewModel>();
		builder.Services.AddSingleton<ItemsPageViewModel>();
		return builder;
	}
}
