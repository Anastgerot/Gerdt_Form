using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Gerdt_Form
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MessageHeader
    {
        public int to;
        public int from;
        public int type;
        public int size;
    }
    public partial class Form1 : Form
    {
        private System.Timers.Timer timer;
        private Process process;
        private int clientID = 0;
        public Form1()
        {
            InitializeComponent();
            //AllocConsole();

            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1 && int.TryParse(args[1], out int id))
            {
                clientID = id;
                this.Text = $"Клиент №{clientID}"; 
            }
        }
        //[DllImport("kernel32.dll")]
        //static extern bool AllocConsole();


        [DllImport(@"C:\Users\anast\OneDrive\Документы\GitHub\Gerdt_SystemProgram3\Gerdt_Form\x64\Debug\Gerdt_DLL.dll", CharSet = CharSet.Unicode)]
        public static extern void sendCommand(int commandId, string message); 


        [DllImport(@"C:\Users\anast\OneDrive\Документы\GitHub\Gerdt_SystemProgram3\Gerdt_Form\x64\Debug\Gerdt_DLL.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr getMessages();

        [DllImport(@"C:\Users\anast\OneDrive\Документы\GitHub\Gerdt_SystemProgram3\Gerdt_Form\x64\Debug\Gerdt_DLL.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr UpdateState(int type = 0);


        private void UpdateListBox()
        {
            IntPtr result = UpdateState();
            if (result != IntPtr.Zero)
            {
                int selectedIndex = ListBox.SelectedIndex;
                int topIndex = ListBox.TopIndex;

                ListBox.BeginUpdate();
                ListBox.Items.Clear();

                ListBox.Items.Add("Все клиенты");

                string resultText = Marshal.PtrToStringUni(result);
                var clientsArray = resultText.Split('|');

                for (var i = 0; i < clientsArray.Length - 1; i++)
                {
                    ListBox.Items.Add("Клиент №" + clientsArray[i]);
                }

                if (selectedIndex >= 0 && selectedIndex < ListBox.Items.Count)
                    ListBox.SelectedIndex = selectedIndex;
                else
                    ListBox.SelectedIndex = 0;

                if (topIndex < ListBox.Items.Count)
                    ListBox.TopIndex = topIndex;

                ListBox.EndUpdate();
            }
        }
        private void UpdateMessagesListBox()
        {
            messagesListBox.Items.Clear();
            IntPtr result = UpdateState(1);
            if (result != IntPtr.Zero)
            {
                string resultText = Marshal.PtrToStringUni(result);
                var clientsArray = resultText.Split('|');

                // Если список клиентов пуст, ничего не показываем
                if (clientsArray.Length == 0 || clientsArray[0] == "none")
                {
                    return;
                }

                bool showAllMessages = clientID != 0;
                foreach (var clientData in clientsArray)
                {
                    var splited = clientData.Split(']');
                    if (splited.Length > 1)
                    {
                        string clientIdStr = splited[0].TrimStart('[');
                        string messageText = splited[1].Trim();

                        int messageClientId;
                        if (int.TryParse(clientIdStr, out messageClientId))
                        {
                            // Если clientID не 0, показываем все сообщения
                            if (showAllMessages || messageClientId == clientID)
                            {
                                messagesListBox.Items.Add($"{messageText}");
                            }
                        }
                    }
                }
            }
        }

        private void Send_Click(object sender, EventArgs e)
        {

            if (TextBox.Text.Length == 0)
            {
                MessageBox.Show("Введите сообщение");
                return;
            }

            if (ListBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите клиента");
                return;
            }

            if (ListBox.Items.Count <= 1)
            {
                MessageBox.Show("Нет доступных клиентов для отправки сообщений");
                return;
            }

            string selectedThread = ListBox.SelectedItem.ToString();
            int clientdId = 0;

            if (selectedThread != "Все клиенты")
            {
                string idStr = selectedThread.Replace("Клиент №", "").Trim();
                if (!int.TryParse(idStr, out clientdId))
                {
                    MessageBox.Show("Не удалось определить ID клиента");
                    return;
                }
            }

            string message = $"{clientdId}|{TextBox.Text}";

            sendCommand(2, message);
            TextBox.Clear();
        }

        private void OnTimeout(Object source, ElapsedEventArgs e)
        {
            UpdateListBox();
            UpdateMessagesListBox();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            sendCommand(0, clientID.ToString());
            Thread.Sleep(500);
            UpdateListBox();

            timer = new System.Timers.Timer(1000);
            timer.Elapsed += OnTimeout;
            timer.AutoReset = true;
            timer.Enabled = true;


        }
    }
}
