using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;

namespace Client;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void ButtonFind_Click(object sender, RoutedEventArgs e)
    {
        if (t1.Text.Length == 0)
        {
            MessageBox.Show("Please enter the postcode", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        using Socket server = new Socket(
            AddressFamily.InterNetwork,
            SocketType.Stream,
            ProtocolType.IP
        );

        IPAddress ipAddress = Dns.GetHostAddresses("localhost")
       .Where(ipAdress => ipAdress.AddressFamily == AddressFamily.InterNetwork)
       .First();
        IPEndPoint endPoint = new IPEndPoint(ipAddress, 2025);
        await server.ConnectAsync(endPoint);

        byte[] bytes = Encoding.UTF8.GetBytes(t1.Text);
        await server.SendAsync(bytes);

        bytes = new byte[4096];
        int received = await server.ReceiveAsync(bytes);
        t2.Text = Encoding.UTF8.GetString(bytes, 0, received);
    }
}