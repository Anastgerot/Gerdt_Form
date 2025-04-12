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
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Gerdt_Form
{
    public struct Message
    {
        public int id;
        public int size;
    }
    public partial class Form1 : Form
    {
        private int totalThreads = 0;
        private Process process;
        public Form1()
        {
            InitializeComponent();
            //AllocConsole();
        }
        //[DllImport("kernel32.dll")]
        //static extern bool AllocConsole();

        [DllImport(@"C:\Users\anast\OneDrive\Документы\GitHub\Gerdt_SystemProgram2\Gerdt_Form\x64\Debug\Gerdt_DLL.dll", CharSet = CharSet.Unicode)]
        public static extern bool initconnect();

        [DllImport(@"C:\Users\anast\OneDrive\Документы\GitHub\Gerdt_SystemProgram2\Gerdt_Form\x64\Debug\Gerdt_DLL.dll", CharSet = CharSet.Unicode)]
        public static extern void sendCommand(int commandId, string message);


        private void Start_Click(object sender, EventArgs e)
        {

            if (!ListBox.Items.Contains("Главный поток"))
                ListBox.Items.Add("Главный поток");

            if (!ListBox.Items.Contains("Все потоки"))
                ListBox.Items.Add("Все потоки");

            int threadCount = (int)NumericUpDown.Value;
            if (threadCount <= 0)
            {
                MessageBox.Show("Укажите верное число потоков");
                return;
            }

            string data = threadCount.ToString();
            int type = 2;

            sendCommand(type, data);

            for (int i = 1; i <= threadCount; i++)
            {
                totalThreads++;
                ListBox.Items.Add($"Поток: {totalThreads}");
            }
        }

        private void Stop_Click(object sender, EventArgs e)
        {

            if (totalThreads == 0)
            {
                MessageBox.Show("Нет потоков для закрытия");
                return;
            }

            int messageType = 3;
            string messageData = "";
            sendCommand(messageType, messageData);

            for (int i = ListBox.Items.Count - 1; i >= 0; i--)
            {
                string item = ListBox.Items[i].ToString();
                if (item.StartsWith("Поток:"))
                {
                    ListBox.Items.RemoveAt(i);
                    totalThreads--;
                    break;
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
                MessageBox.Show("Выберите поток");
                return;
            }

            if (ListBox.Items.Count == 2)
            {
                MessageBox.Show("Нет доступных потоков для отправки сообщений");
                return;
            }

            string selectedThread = ListBox.SelectedItem.ToString();
            int threadId = 0;

            if (selectedThread == "Главный поток")
            {
                threadId = -1;
            }
            else if (selectedThread == "Все потоки")
            {
                threadId = 0;
            }
            else
            {
                if (!int.TryParse(new string(selectedThread.Where(char.IsDigit).ToArray()), out threadId))
                {
                    MessageBox.Show("Ошибка: Некорректный выбор потока");
                    return;
                }
            }

            string message = TextBox.Text;
            int messageType = 4; 

            string fullMessage = $"{threadId}|{message}";

            sendCommand(messageType, fullMessage);
            MessageBox.Show("Сообщение отправлено");

        }
    }
}
