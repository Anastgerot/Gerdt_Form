using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Gerdt_Form
{
    public enum MessageTypes : int
    {
        MT_INIT,
        MT_EXIT,
        MT_GETDATA,
        MT_DATA,
        MT_UPDATE,
        MT_UPDATE_MESSAGES,
        MT_NODATA,
        MT_CONFIRM
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MessageHeader
    {
        [MarshalAs(UnmanagedType.I4)]
        public int to;
        [MarshalAs(UnmanagedType.I4)]
        public int from;
        [MarshalAs(UnmanagedType.I4)]
        public int type;
        [MarshalAs(UnmanagedType.I4)]
        public int size;
    }

    public class Message
    {
        public MessageHeader header;
        public byte[] data;

        public Message(MessageHeader _header, string _data = "")
        {
            this.header = _header;
            this.data = Encoding.Unicode.GetBytes(_data);
        }

        public string GetStringData()
        {
            return Encoding.Unicode.GetString(data);
        }
    }

    public class Session
    {
        public Socket socket;
        public bool isConnected = false;
        public int sessionId = 0;

        public bool connect()
        {
            if (this.socket != null && this.socket.Connected)
            {
                MessageBox.Show("Уже подключен");
            }
            try
            {
                int nPort = 12345;
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), nPort);
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(endPoint);

                MessageHeader mHeader = new MessageHeader
                {
                    to = 0,
                    from = 0,
                    type = (int)MessageTypes.MT_INIT,
                    size = 0
                };
                send(new Message(mHeader));

                byte[] idBuffer = new byte[4];
                receiveFully(idBuffer, 4);
                sessionId = BitConverter.ToInt32(idBuffer, 0);

                if (!socket.Connected)
                    return false;

                isConnected = socket.Connected;
                return isConnected;
            }
            catch (Exception e)
            {
                MessageBox.Show("Ошибка подключения: " + e.Message);
                return false;
            }
        }

        public bool disconnect()
        {
            try
            {
                MessageHeader mHeader = new MessageHeader
                {
                    to = 0,
                    from = sessionId,
                    type = (int)MessageTypes.MT_EXIT,
                    size = 0
                };
                send(new Message(mHeader));

                socket.Shutdown(SocketShutdown.Both);
                socket.Close();

                isConnected = false;
                sessionId = 0;
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("Ошибка отключения: " + e.Message);
                return false;
            }
        }

        public void send(Message message)
        {
            if (socket == null || !socket.Connected)
                return;

            byte[] dataBytes = message.data ?? new byte[0];
            message.header.size = dataBytes.Length;

            byte[] headerBytes = toBytes(message.header);
            socket.Send(headerBytes, SocketFlags.None);
            if (message.header.size > 0)
            {
                socket.Send(dataBytes, SocketFlags.None);
            }
        }

        public Message receive()
        {
            int headerSize = Marshal.SizeOf(typeof(MessageHeader));
            byte[] headerBuffer = new byte[headerSize];

            int received = socket.Receive(headerBuffer, headerSize, SocketFlags.None);
            if (received == 0)
                return new Message(new MessageHeader { type = (int)MessageTypes.MT_NODATA });

            MessageHeader header = fromBytes<MessageHeader>(headerBuffer);

            byte[] dataBuffer = new byte[header.size];
            if (header.size > 0)
            {
                receiveFully(dataBuffer, header.size);
            }

            return new Message(header, Encoding.Unicode.GetString(dataBuffer));
        }

        public string get(MessageTypes mType)
        {
            MessageHeader mHeader = new MessageHeader
            {
                to = 0,
                from = sessionId,
                type = (int)mType,
                size = 0
            };

            send(new Message(mHeader));

            Message response = receive();

            if (response.header.size == 0)
            {
                MessageBox.Show("Получен пустой ответ (длина = 0) от сервера на " + mType.ToString());
                return "";
            }

            string result = Encoding.Unicode.GetString(response.data);
            MessageBox.Show($"Получены данные ({mType}): \"{result}\"");

            return result;
        }
        private void receiveFully(byte[] buffer, int length)
        {
            int received = 0;
            while (received < length)
            {
                int bytes = socket.Receive(buffer, received, length - received, SocketFlags.None);
                if (bytes == 0)
                    throw new SocketException();

                received += bytes;
            }
        }

        // Универсальный метод для преобразования структуры в байты
        static byte[] toBytes(object obj)
        {
            int size = Marshal.SizeOf(obj);
            byte[] buff = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(obj, ptr, true);
            Marshal.Copy(ptr, buff, 0, size);
            Marshal.FreeHGlobal(ptr);
            return buff;
        }

        static T fromBytes<T>(byte[] buff) where T : struct
        {
            T data = default(T);
            int size = Marshal.SizeOf(data);
            IntPtr i = Marshal.AllocHGlobal(size);
            Marshal.Copy(buff, 0, i, size);
            var d = Marshal.PtrToStructure(i, data.GetType());
            if (d != null)
            {
                data = (T)d;
            }
            Marshal.FreeHGlobal(i);
            return data;
        }

        ~Session()
        {
            if (socket != null && socket.Connected)
            {
                disconnect();
            }
        }
    }
}
