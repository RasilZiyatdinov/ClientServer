using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp
{
    public class Client
    {
        private int port;
        private string ipAdress;
        TcpClient tcpClient;
        public Client(string ipAddress, int p)
        {
            ipAdress = ipAddress;
            port = p;
        }

        //отправка запроса на сервер и получение ответа
        public async Task<string> ExchangeAsync(MyFile file)
        {
            tcpClient = new TcpClient(ipAdress, port);
            var stream = tcpClient.GetStream();
            try
            {
                //отправка строки на сервер
                using StreamWriter writer = new StreamWriter(stream, leaveOpen: true);                
                await writer.WriteLineAsync(file.FileContent);
                await writer.FlushAsync();

                //получение ответа
                using StreamReader reader = new StreamReader(stream, leaveOpen: true);
                string answer = await reader.ReadLineAsync();

                return answer;
            }
            finally
            {
                stream.Close();
            }
        }
    }
}
