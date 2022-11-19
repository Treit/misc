using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace TaskDeadlockExample
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        void SyncOverAsyncClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(AsyncTest.DoSomethingAsync().Result);
        }

        void ConfigureAwaitClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(AsyncTest.DoSomethingConfigureAwaitAsync().Result);
        }

        void SyncClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(AsyncTest.DoSomething());
        }

        async void AsyncClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(await AsyncTest.DoSomethingAsync());
        }
    }

    static class AsyncTest
    {
        internal static string DoSomething()
        {
            Thread.Sleep(50);
            return "Sweet!";
        }
        internal static async Task<string> DoSomethingAsync()
        {
            await Task.Delay(50);
            return "Nice one!";
        }

        internal static async Task<string> DoSomethingConfigureAwaitAsync()
        {
            await Task.Delay(50).ConfigureAwait(false);
            return "Right on!";
        }
    }
}
