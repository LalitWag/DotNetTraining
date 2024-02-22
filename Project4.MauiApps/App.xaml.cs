using Project4.MauiApps.Views;

namespace Project4.MauiApps
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new OurStudents());
           // MainPage = new AppShell();
            
        }
    }
}
