// #define MOCK
using Prism;
using Prism.Ioc;
using DevConsultHack.Client.ViewModels;
using DevConsultHack.Client.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Prism.Unity;
using DevConsultHack.Client.Repositories;
using DevConsultHack.Client.Services;
using System.Diagnostics;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace DevConsultHack.Client
{
    public partial class App : PrismApplication
    {
        /* 
         * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
         * This imposes a limitation in which the App class must have a default constructor. 
         * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
         */
        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer) { }

        protected override async void OnInitialized()
        {
            InitializeComponent();
            await NavigationService.NavigateAsync("AppNavigationPage/MainPage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
#if MOCK
            containerRegistry.RegisterSingleton<IPictureRepository, MockPictureRepository>();
#else
            containerRegistry.RegisterSingleton<IPictureRepository, AzurePictureRepository>();
#endif
            containerRegistry.RegisterSingleton<Settings>();
            containerRegistry.RegisterSingleton<IMediaService, MediaService>();
            containerRegistry.RegisterForNavigation<AppNavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage>();
            containerRegistry.RegisterForNavigation<HistoryPage>();
            containerRegistry.RegisterForNavigation<AppNavigationPage>();
        }
    }
}
