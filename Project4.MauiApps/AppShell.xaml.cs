using Project4.MauiApps.Views;

namespace Project4.MauiApps
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(OurStudents), typeof(OurStudents));
            Routing.RegisterRoute(nameof(AddPage), typeof(AddPage));
            Routing.RegisterRoute(nameof(EditPage), typeof(EditPage));
        }
    }
}
