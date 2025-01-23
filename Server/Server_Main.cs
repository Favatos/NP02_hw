using System.ComponentModel.Design.Serialization;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server;

internal class Server_Main
{
    private static Dictionary<int, List<string>> indexToStreets = new();
    static async Task Main(string[] args)
    {
        using TcpListener listener = new(IPAddress.Any, 2025);
        listener.Start();
        Console.WriteLine("Server is runnning and wating for incoming connections");
        GetStreets("streets.txt");

        while (true)
        {
            using TcpClient client = await listener.AcceptTcpClientAsync();
            Console.WriteLine($"Client {client.Client.RemoteEndPoint} has connected");

            byte[] bytes = new byte[4096];
            int received = await client.GetStream().ReadAsync(bytes);
            int index = int.Parse(Encoding.UTF8.GetString(bytes, 0, received));

            List<string> value = new ();
            string streets = indexToStreets.TryGetValue(index, out value) ? String.Join("\n", value.ToArray()) : "postcode not found";

            bytes = Encoding.UTF8.GetBytes(streets);
            await client.GetStream().WriteAsync(bytes);
        }
    }

    private static void GetStreets(string path)
    {
        using TextReader reader = File.OpenText(path);
        string? line;
        int i2 = 0;
        while ((line = reader.ReadLine()) is not null)
        {
            if (int.TryParse(line, out int index))
            {
                i2 = index;
                indexToStreets.Add(index, new List<string>());
                continue;
            }

            indexToStreets[i2].Add(line);
        }
    }
}
