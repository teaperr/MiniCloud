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
                DirectoryTreeView.ItemsSource = GetStructure(structure);
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
                string ownerName= xmlOwner.GetAttribute("name");
                var owner= new MCOwner(ownerName);
                owners.Add(owner);
                owner.Structures=GetDirectoriesStructure(xmlOwner, ownerName);
            }
            return owners;
            
        }
        private ICollection<MCStructure> GetDirectoriesStructure(XmlElement xmlOwner, string ownerName)
        {
            var result=new List<MCStructure>();
            foreach (XmlElement child in xmlOwner.ChildNodes)
            {
                string attributeName= child.GetAttribute("name");
                string path = $"{attributeName}";
                var directory=new MCStructure(attributeName,path,ownerName, false);
                GenerateDirectoryStructure(directory,ownerName, child);
                result.Add(directory);
            }
            return result;
        }
        private void GenerateDirectoryStructure(MCStructure parentDir, string ownerName, XmlElement parentNode)
        {
            foreach (XmlElement child in parentNode.ChildNodes)
            {
                var attributeName= child.GetAttribute("name");
                var path= $"{parentDir.Path}\\{attributeName}";
                if (child.Name == "file")
                {
                    parentDir.Structures.Add(new MCStructure(attributeName, path, ownerName, true));
                }
                else
                {
                    var directory= new MCStructure(attributeName, path,ownerName,false);
                    parentDir.Structures.Add(directory);
                    GenerateDirectoryStructure(directory, ownerName, child);
                }

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
            //LoginButton.IsEnabled = enable;
            //RegisterButton.IsEnabled = enable;
            //LoginTextBox.IsEnabled = enable;
            //PasswordTextBox.IsEnabled = enable;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }

        private void DirectoryTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem SelectedItem = DirectoryTreeView.SelectedItem as TreeViewItem;
            if(e.NewValue is MCStructure)
            {
                var structure = e.NewValue as MCStructure;
                if(structure.IsFile)
                    DirectoryTreeView.ContextMenu = DirectoryTreeView.Resources["FileContext"] as ContextMenu;
                else
                DirectoryTreeView.ContextMenu = DirectoryTreeView.Resources["DirectoryContext"] as ContextMenu;
            }
        }

        private void UploadFileHere(object sender, RoutedEventArgs e)
        {
            var directory= DirectoryTreeView.SelectedItem as MCStructure;

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            bool? openedFile = dlg.ShowDialog();
            if (openedFile != true)
                return;
            var base64file=Convert.ToBase64String(File.ReadAllBytes(dlg.FileName));

            var fileName= System.IO.Path.GetFileName(dlg.FileName);
            

            var request = $"file upload {directory.Path} {fileName} {base64file}";
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
                    MessageBox.Show($"Nie udało się wysłać pliku.\n{result}");
                }
                else
                    MessageBox.Show("OK");
            };
        }
    }
}
