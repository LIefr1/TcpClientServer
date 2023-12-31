﻿using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace server
{
	/// <summary>
	/// Сервер.
	/// </summary>
	public class Server
	{
		/// <summary>
		/// Сокет сервера.
		/// </summary>
		private Socket _socket;

		/// <summary>
		/// Список клиентов.
		/// </summary>
		private List<Socket> _clients;

		/// <summary>
		/// Размер буфера приема сообщений.
		/// </summary>
		private const int BufferSize = 1024;

		/// <summary>
		/// Набор некорректныз данных.
		/// </summary>
		private char[] _invalidSymbols;

		public Server(Socket socket)
		{
			if (socket == null)
			{
				throw new ArgumentNullException(nameof(socket), "Пустой сокет.");
			}

			_socket = socket;
			_socket.Bind(new IPEndPoint(IPAddress.Any, 9090));

			_clients = new List<Socket>();
			_invalidSymbols = new char[] { '/', '<', '>', '^', ';', '{', '}', '`', '~' };
		}


		/// <summary>
		/// Запуск сервера.
		/// </summary>
		public void Start()
		{
			_socket.Listen();

			while (true)
			{
				var client_socket = _socket.Accept();
				_clients.Add(client_socket);
				new Thread(() => ListenClient(client_socket)).Start();
			}
		}


		/// <summary>
		/// Слушаем клиента.
		/// </summary>
		/// <param name="client">Клиент.</param>
		private void ListenClient(Socket client)
		{
			if (client == null)
			{
				return;
			}

			while (true)
			{
				var buffer = new byte[BufferSize];

				int bytesRecv = client.Receive(buffer);
				var data = Encoding.UTF8.GetString(buffer, 0, bytesRecv);

				// Если пользователь прислал пустую строку.
				if (String.IsNullOrWhiteSpace(data))
				{
					continue;
				}

				var separetedMessage = SeparateMessage(data);

				// Если флаг содержит некорректные символы.
				if (!CheckDataIsCorrect(separetedMessage.Flag))
				{
					continue;
				}

				if (separetedMessage.Flag.ToLower() == "lower")
				{
					client.Send(Encoding.UTF8.GetBytes(separetedMessage.Message.ToLower()));
				}
				else if (separetedMessage.Flag.ToLower() == "upper")
				{
					client.Send(Encoding.UTF8.GetBytes(separetedMessage.Message.ToUpper()));
				}
			}
		}

		
		/// <summary>
		/// Разделяет сообщение на флаг и само сообщение.
		/// </summary>
		/// <param name="message">Целое сообщение клиента.</param>
		/// <returns>Разделенное сообщение.</returns>
		private SeparatedMessage SeparateMessage(string message)
		{
			var separatedMessage = new SeparatedMessage();

			var splitMessage = message.Split(' ');

			separatedMessage.Flag = splitMessage[0].Substring(1);
			separatedMessage.Message = String.Join(" ", splitMessage, 1, splitMessage.Length - 1);

			// Example:
			// Flag: lower, Message:Hello
			return separatedMessage;
		}


		/// <summary>
		/// Проверка корректности строки.
		/// </summary>
		/// <param name="data">Проверяемая строка.</param>
		/// <returns>True, если строка корректная.</returns>
		private bool CheckDataIsCorrect(string data)
		{
			foreach (var symbol in _invalidSymbols)
			{
				if (data.Contains(symbol))
				{
					return false;
				}
			}

			return true;
		}
	}
}
