using server;
using System.Net;
using System.Net.Sockets;
using System.Text;

var server = new Server(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp));
server.Start();