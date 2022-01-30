using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public class Server
    {
        private bool IsRun; //вкл и выкл сервера         
        List<Task> allTasks; //список задач - обрабатываемые запросы

        private TcpListener server;
        private SemaphoreSlim throttler; //ограничитель на количество одновременно обрабатываемых запросов
        private int n;
        public int N //количество одновременно обрабатываемых запросов
        {
            get => n;
            set
            {
                if (value > 0)
                    n = value;
                else
                {
                    Console.WriteLine("Incorrect N");
                }
            }     
        }

        public Server(string ipAdress, int port, int number)
        {
            server = new TcpListener(IPAddress.Parse(ipAdress), port);
            IsRun = true;
            N = number;
            allTasks = new List<Task>();
            throttler = new SemaphoreSlim(initialCount: N);
        }

        //запуск прослушивания
        public async Task ListenAsync()
        {       
            server.Start();
            Console.WriteLine($"Started listening requests at: {server.LocalEndpoint}, protocol type: {ProtocolType.Tcp}");
            try
            {
                while (IsRun)
                {
                    TcpClient client = await server.AcceptTcpClientAsync();

                    if (throttler.CurrentCount < 1)
                    {
                        _ = ProcessError(client);
                        //await Task.WhenAll(allTasks);
                    }
                    else
                    {
                        await throttler.WaitAsync();
                        allTasks.Add(_ = ProcessRequestAsync(client));
                    }
                }
            }
            finally
            {
                await Task.WhenAll(allTasks);
                server.Stop();
                Console.WriteLine("Server is finished");
            }
        }

        //ответ на запрос ошибкой
        public async Task ProcessError(TcpClient tcpClient)
        {
            NetworkStream stream = tcpClient.GetStream();
            using StreamWriter writer = new StreamWriter(stream);
            await writer.WriteLineAsync("Сервер перегружен");
            await writer.FlushAsync();
        }

        //обработка запроса
        public async Task ProcessRequestAsync(TcpClient tcpClient)
        {
            //await Task.Yield();
            try
            {
                NetworkStream stream = tcpClient.GetStream();

                //получение данных клиента
                using StreamReader reader = new StreamReader(stream, leaveOpen: true);
                string line = await reader.ReadLineAsync();
                await Console.Out.WriteLineAsync($"{DateTime.Now}: Получена строка \"{line}\". Запрос обрабатывается...");

                await Task.Delay(5000); //задержка обработки запроса на 5 сек

                //формирование и отправка ответа клиенту
                using StreamWriter writer = new StreamWriter(stream, leaveOpen: true);
                if (IsPalindrom(line))
                    await writer.WriteLineAsync("Палиндром");
                else
                    await writer.WriteLineAsync("Не палиндром");
                await writer.FlushAsync();

                //уведомление на сервере
                await Console.Out.WriteLineAsync($"{DateTime.Now}: Строка \"{line}\" обработана");
            }
            finally
            {
                throttler.Release(); //освобождение семафора
            }
        }

        //проверка: является ли строка палиндромом
        static bool IsPalindrom(string str, bool ignoreCase = true, bool ignorSpace = true)
        {
            if (ignoreCase)
                str = str.ToLower();
            if (ignorSpace)
                str = str.Replace(" ", string.Empty);

            for (int first = 0, last = str.Length - 1; first < last; ++first, --last)
            {
                if (str[first] != str[last])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
