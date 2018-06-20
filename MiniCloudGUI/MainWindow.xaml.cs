using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace MiniCloudGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private static readonly Socket ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private readonly BackgroundWorker socketWorker = new BackgroundWorker();
        private const int PORT = 100;
        private const int BUFFER_SIZE = 50 * 1000 * 1000;
        private readonly byte[] buffer = new byte[BUFFER_SIZE];
        private bool connectedToServer=false;

        public MainWindow()
        {
            InitializeComponent();
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                ConnectToServer();
            }).Start();
        }
        private void ConnectToServer()
        {
            while (!ClientSocket.Connected)
            {
                try
                {
                    ClientSocket.Connect(IPAddress.Loopback, PORT);
                    connectedToServer = true;
                }
                catch (SocketException)
                {

                }
            }
        }
        private void SendRequest(string request)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(request);
            ClientSocket.Send(buffer);
        }
        private string ReceiveResponse()
        {
            string text;
            int received = ClientSocket.Receive(buffer, SocketFlags.None);
            if (received == 0)
                return "";
            var data = new byte[received];
            Array.Copy(buffer, data, received);
            text = Encoding.ASCII.GetString(data);
            return text;
        }
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string login=LoginTextBox.Text;
            string password=PasswordTextBox.Text;
            if(connectedToServer==false)
            {
                MessageBox.Show("Niestety nie udało sie połączyć z serwerem");
                return;
            }
            EnableLoginPanel(false);

            var request=$"account login {login} {password}";
            string result=null;

            var backgroundWork=new BackgroundWorker();
            backgroundWork.DoWork += (s, e1) =>
            {
                SendRequest(request);
                result = ReceiveResponse();
            };
            backgroundWork.RunWorkerAsync();
            backgroundWork.RunWorkerCompleted += (s, e1) =>
            {
                if(result.StartsWith("_ERROR_"))
                {
                    MessageBox.Show("Nie udało się zalogować.");
                    EnableLoginPanel(true);
                }
                else
                {
                    MessageBox.Show("Zalogowano");
                    LoadDirectoryStructure();

                }
            };
        }

        private void LoadDirectoryStructure()
        {
            var request = $"directory structure";
            string result = null;

            var backgroundWork = new BackgroundWorker();
            backgroundWork.DoWork += (s, e1) =>
            {
                SendRequest(request);
                result = ReceiveResponse();
            };
            backgroundWork.RunWorkerAsync();
            backgroundWork.RunWorkerCompleted += (s, e1) =>
            {
                var structure = result.Substring(6);
                FillDirectoryTree(structure);
            };
        }
        private IEnumerable<MCOwner> GetStructure(string structure)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(structure);
            var xmlOwners = xml.FirstChild.ChildNodes;
            var owners = new List<MCOwner>();
            foreach (XmlElement xmlOwner in xmlOwners)
            {
                var owner= new MCOwner(xmlOwner.GetAttribute("name"));
                owners.Add(owner);
                owner.Directories=GetDirectoriesStructure(xmlOwner);
            }
            return owners;
            
        }

        private ICollection<MCDirectory> GetDirectoriesStructure(XmlElement xmlOwner)
        {
            foreach (XmlElement child in rootDirectory.ChildNodes)
            {
                var item = new TreeViewItem();
                if (child.Name == "file")
                {
                    var fileName = child.GetAttribute("name");
                    if (fileName.Split('.').Count() == 1)
                    {
                        fileName += ".file";
                        item.Header = fileName;
                        rootItem.Items.Add(item);
                        continue;
                    }
                }
                item.Header = child.GetAttribute("name");
                rootItem.Items.Add(item);
                AddDirectoriesToTreeView(item, child);
            }
        }

        private void FillDirectoryTree(string structure)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(structure);
            var owners=xml.FirstChild.ChildNodes;
            foreach(XmlElement owner in owners)
            {
                var ownerItem=new TreeViewItem();
                ownerItem.Header=owner.GetAttribute("name");
                foreach(XmlElement directory in owner.ChildNodes)
                {
                    AddDirectoriesToTreeView(ownerItem, directory);
                }
                DirectoryTreeView.Items.Add(ownerItem);
            }
        }

        private void AddDirectoriesToTreeView(TreeViewItem rootItem, XmlElement rootDirectory)
        {
            foreach (XmlElement child in rootDirectory.ChildNodes)
            {
                var item = new TreeViewItem();
                if(child.Name=="file")
                {
                    var fileName=child.GetAttribute("name");
                    if(fileName.Split('.').Count()==1)
                    {
                        fileName+=".file";
                        item.Header = fileName;
                        rootItem.Items.Add(item);
                        continue;
                    }
                }
                item.Header = child.GetAttribute("name");
                rootItem.Items.Add(item);
                AddDirectoriesToTreeView(item, child);
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            string password = PasswordTextBox.Text;
            if (connectedToServer == false)
            {
                MessageBox.Show("Niestety nie udało sie połączyć z serwerem");
                return;
            }
            EnableLoginPanel(false);

            var request = $"account register {login} {password}";
            string result = null;

            var backgroundWork = new BackgroundWorker();
            backgroundWork.DoWork += (s, e1) =>
            {
                SendRequest(request);
                result = ReceiveResponse();
            };
            backgroundWork.RunWorkerAsync();
            backgroundWork.RunWorkerCompleted += (s, e1) =>
            {
                if (result.StartsWith("_ERROR_"))
                {
                    MessageBox.Show($"Nie udało się zarejstrować.\n{result}");
                    EnableLoginPanel(true);
                }
                else
                {
                    MessageBox.Show("Zarejestrowano pomyślnie.\nMożna się logować.");
                    EnableLoginPanel(true);
                }
            };
        }
        private void EnableLoginPanel(bool enable)
        {
            LoginButton.IsEnabled = enable;
            RegisterButton.IsEnabled = enable;
            LoginTextBox.IsEnabled = enable;
            PasswordTextBox.IsEnabled = enable;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var structureFromFile = File.ReadAllText("structure.txt");
            var ownerList=new List<MCOwner>()
            {
                new MCOwner() { Name="xxx"},
                new MCOwner() {Name="yyy"}
            };
            ownerList.First().Directories.Add(new MCDirectory() { Name="aaa"});
            DirectoryTreeView.ItemsSource=ownerList;
            //FillDirectoryTree(structureFromFile);
        }
    }
}
