using client;
using System.Net;
using System.Net.Sockets;
using System.Text;

var client = new Client(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp));

try
{
	client.ConnectToServer("127.0.0.1", 9090);
	client.StartListenServer();

	while (true)
	{
		var message = Console.ReadLine();

		if (message == "quit")
		{
			break;
		}

		client.SendMessage(message);
	}
}
catch (SocketException ex)
{
	Console.WriteLine(ex.Message);
}

Console.ReadKey();
