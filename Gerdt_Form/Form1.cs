using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using static System.Collections.Specialized.BitVector32;

namespace Gerdt_Form
{ 
    public partial class Form1 : Form
    {

        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        private System.Timers.Timer timer;
        private int clientID = 0;
        private Session session;
        private DateTime lastActivityTime = DateTime.Now;

        public Form1()
        {
            AllocConsole();

            InitializeComponent();

            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1 && int.TryParse(args[1], out int id))
            {
                clientID = id;
            }

            this.Text = $"Клиент №{clientID}";
        }

        private void RegisterActivity()
        {
            lastActivityTime = DateTime.Now;
        }

        private void UpdateListBox()
        {
            ListBox.BeginUpdate();
            ListBox.Items.Clear();
            ListBox.Items.Add("Все клиенты");

            string response = session.get(MessageTypes.MT_UPDATE);

            MessageBox.Show(response);

            if (!string.IsNullOrWhiteSpace(response))
            {
                var clients = response.Split('|');
                foreach (var c in clients)
                {
                    if (!string.IsNullOrWhiteSpace(c))
                        ListBox.Items.Add("Клиент №" + c);
                }
            }

            if (ListBox.Items.Count > 0)
                ListBox.SelectedIndex = 0;

            ListBox.EndUpdate();
           
        }

        private void UpdateMessagesListBox()
        {
            try
            {
                messagesListBox.Items.Clear();
                string response = session.get(MessageTypes.MT_UPDATE_MESSAGES);

                if (string.IsNullOrWhiteSpace(response) || response == "none")
                    return;

                var lines = response.Split('|');
                foreach (var line in lines)
                {
                    var parts = line.Split(']');
                    if (parts.Length == 2)
                    {
                        string clientIdStr = parts[0].TrimStart('[');
                        string messageText = parts[1].Trim();

                        if (int.TryParse(clientIdStr, out int toId))
                        {
                            if (toId == 0 || toId == session.sessionId)
                            {
                                messagesListBox.Items.Add(messageText);
                            }
                        }
                    }
                }
            }
            catch { }
        }

        private void Send_Click(object sender, EventArgs e)
        {
            RegisterActivity();

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

            string selected = ListBox.SelectedItem.ToString();
            int to = 0;
            if (selected != "Все клиенты")
            {
                if (!int.TryParse(selected.Replace("Клиент №", "").Trim(), out to))
                {
                    MessageBox.Show("Неверный формат клиента");
                    return;
                }
            }

            var header = new MessageHeader
            {
                to = to,
                type = 3,
                size = 0
            };

            session.send(new Message(header, TextBox.Text));
            TextBox.Clear();

        }

        private void OnTimeout(object source, ElapsedEventArgs e)
        {
            //if ((DateTime.Now - lastActivityTime).TotalSeconds >= 20)
            //{
            //    this.Invoke((MethodInvoker)delegate
            //    {
            //        timer.Stop();
            //        MessageBox.Show("Клиент неактивен более 20 секунд. Окно будет закрыто.");
            //        session.disconnect();
            //        this.Close();
            //    });
            //    return;
            //}

            //this.Invoke((MethodInvoker)delegate
            //{
            //    UpdateListBox();
            //    UpdateMessagesListBox();
            //});
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            session = new Session();

            this.MouseMove += (_, __) => RegisterActivity();
            this.MouseClick += (_, __) => RegisterActivity();
            this.KeyDown += (_, __) => RegisterActivity();
            TextBox.TextChanged += (_, __) => RegisterActivity();

            timer = new System.Timers.Timer(1000);
            timer.Elapsed += OnTimeout;
            timer.AutoReset = true;
            timer.Enabled = false; 

        }

        private void Form1Closing(object sender, FormClosingEventArgs e)
        {
            session?.disconnect();
        }

        private void Connect_Click(object sender, EventArgs e)
        {
            if (!session.connect())
            {
                MessageBox.Show("Сервер не найден");
                return;
            }

            timer.Start();
            this.Text = "Клиент №" + session.sessionId;
            UpdateListBox();
            //UpdateMessagesListBox();

        }

        private void Disconnect_Click(object sender, EventArgs e)
        {
            messagesListBox.Items.Clear();
            session.disconnect();
            timer.Stop();
            this.Text = "Клиент (отключён)";

        }
    }
}

