using System;
using System.Net.Sockets;
using System.Text;

namespace client
{
	/// <summary>
	/// Клиент.
	/// </summary>
	public class Client
	{
		/// <summary>
		/// Сокет клиента.
		/// </summary>
		private Socket _socket;

		/// <summary>
		/// Размер буфера приема сообщений.
		/// </summary>
		private const int BufferSize = 1024;

		public Client(Socket socket)
		{
			if (socket == null)
			{
				throw new ArgumentNullException(nameof(socket), "Пустой сокет.");
			}

			_socket = socket;
		}


		/// <summary>
		/// Соединение с сервером.
		/// </summary>
		/// <param name="ip">IP сервера.</param>
		/// <param name="port">Порт сервера.</param>
		public void ConnectToServer(string ip, int port)
		{
			try
			{
				_socket.Connect(ip, port);
			}
			catch (SocketException ex)
			{
				throw ex;
			}
		}


		/// <summary>
		/// Слушаем сервер.
		/// </summary>
		public void StartListenServer()
		{
			new Thread(() =>
			{
				var buffer = new byte[BufferSize];

				while (true)
				{
					int recvBytes = _socket.Receive(buffer);
					string answer = Encoding.UTF8.GetString(buffer, 0, recvBytes);
					Console.WriteLine(answer);
				}
			}).Start();
		}


		/// <summary>
		/// Отправка сообещния.
		/// </summary>
		/// <param name="message">Сообщение.</param>
		public void SendMessage(string message)
		{
			var bytesMessage = Encoding.UTF8.GetBytes(message);

			try
			{
				_socket.Send(bytesMessage);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}
	}
}
