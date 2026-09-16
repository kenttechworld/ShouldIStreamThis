using Dark.Net;
using ShouldIStreamThis.Handlers;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ShouldIStreamThis
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        AppStartUpHandler startupHandler = new();
        public MainWindow()
        {
            InitializeComponent();
            DarkNet.Instance.SetWindowThemeWpf(this, Theme.Dark);
            startupHandler.StartupChecks();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Forces the entire WPF application process to terminate immediately,
            // shutting down any hidden/dangling child windows.
            Application.Current.Shutdown();
        }
    }
}