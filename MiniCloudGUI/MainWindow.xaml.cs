using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using WpfTutorialSamples.Dialogs;

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
        private string loggedUserName=null;

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
            var md5request=$"{CheksumGenerator.CreateMD5(request)} {request}";
            byte[] buffer = Encoding.ASCII.GetBytes(md5request);
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
            var cleanText= text.Substring(33);
            var md5=text.Substring(0,32);
            if(CheksumGenerator.CreateMD5(cleanText)!=md5)
            {
                MessageBox.Show("Odebrano niekompletne dane");
                return null;
            }
            return cleanText;
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
                EnableLoginPanel(true);
                if (result.StartsWith("_ERROR_"))
                {
                    MessageBox.Show(result);
                }
                else
                {
                    UpdateStructure();
                    loggedUserName = login;
                }
            };
        }

        private void UpdateStructure()
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
                DirectoryTreeView.ItemsSource = MCStructureGenerator.GetStructure(structure);
            };
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
                EnableLoginPanel(true);
                if (result.StartsWith("_ERROR_"))
                {
                    MessageBox.Show(result);
                }
                else
                {
                    MessageBox.Show("Zarejestrowano pomyślnie.\nMożna się logować.");
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
        }

        private void DirectoryTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem SelectedItem = DirectoryTreeView.SelectedItem as TreeViewItem;
            DirectoryTreeView.ContextMenu = null;
            if (e.NewValue is MCStructure)
            {
                var structure = e.NewValue as MCStructure;
                if(structure.IsFile)
                {
                    if(structure.OwnerName==loggedUserName)
                        DirectoryTreeView.ContextMenu = DirectoryTreeView.Resources["FileContext"] as ContextMenu;
                    else
                        DirectoryTreeView.ContextMenu = DirectoryTreeView.Resources["SharedFileContext"] as ContextMenu;
                }
                else if(structure.OwnerName==loggedUserName)
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

            var fileName= System.IO.Path.GetFileName(dlg.FileName).Replace(" ","_");
            
            string result = null;

            var backgroundWork = new BackgroundWorker();
            backgroundWork.DoWork += (s, e1) =>
            {
                var base64file = Convert.ToBase64String(File.ReadAllBytes(dlg.FileName));
                var request = $"file upload {directory.Path} {fileName} {base64file}";
                SendRequest(request);
                result = ReceiveResponse();
            };
            backgroundWork.RunWorkerAsync();
            backgroundWork.RunWorkerCompleted += (s, e1) =>
            {
                if(result==null)
                    return;
                if (result.StartsWith("_ERROR_"))
                {
                    MessageBox.Show(result);
                }
                else
                    UpdateStructure();
            };
        }

        private void DownloadFile(object sender, RoutedEventArgs e)
        {
            var file = DirectoryTreeView.SelectedItem as MCStructure;

            var dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName=file.Name;
            bool? savedFile = dlg.ShowDialog();
            if (savedFile != true)
                return;
            var filePath = dlg.FileName;
            string result = null;

            var backgroundWork = new BackgroundWorker();
            backgroundWork.DoWork += (s, e1) =>
            {
                var request = $"file download {file.OwnerName} {file.Path}";
                SendRequest(request);
                result = ReceiveResponse();
            };
            backgroundWork.RunWorkerAsync();
            backgroundWork.RunWorkerCompleted += (s, e1) =>
            {
                if (result == null)
                    return;
                if (result.StartsWith("_ERROR_"))
                {
                    MessageBox.Show(result);
                }
                else
                {
                    var fileBytes=Convert.FromBase64String(result.Substring(6));
                    File.WriteAllBytes(filePath,fileBytes);
                }
            };
        }

        private void CreateDirectory(object sender, RoutedEventArgs e)
        {
            var directory = DirectoryTreeView.SelectedItem as MCStructure;
            var inputDialog=new InputDialog("Nazwa folderu");
            string newDirectoryName=null;
            if (inputDialog.ShowDialog() == true)
                newDirectoryName = inputDialog.Answer;
            else
                return;
            string result = null;

            var backgroundWork = new BackgroundWorker();
            backgroundWork.DoWork += (s, e1) =>
            {
                var request = $"directory create {directory.Path} {newDirectoryName}";
                SendRequest(request);
                result = ReceiveResponse();
            };
            backgroundWork.RunWorkerAsync();
            backgroundWork.RunWorkerCompleted += (s, e1) =>
            {
                if (result == null)
                    return;
                if (result.StartsWith("_ERROR_"))
                {
                    MessageBox.Show(result);
                }
                else
                    UpdateStructure();
            };

        }

        private void Remove(object sender, RoutedEventArgs e)
        {
            var item = DirectoryTreeView.SelectedItem as MCStructure;
            string request=null;
            if(item.IsFile)
                request= $"file remove {item.Path}";
            else
                request= $"directory remove {item.Path}";

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
                if (result == null)
                    return;
                if (result.StartsWith("_ERROR_"))
                {
                    MessageBox.Show(result);
                }
                else
                    UpdateStructure();
            };
        }
        private void Share(object sender, RoutedEventArgs e)
        {
            var item = DirectoryTreeView.SelectedItem as MCStructure;
            var inputDialog = new InputDialog("Podaj nazwę użytkownika");
            string userName = null;
            if (inputDialog.ShowDialog() == true)
                userName = inputDialog.Answer;
            string request = $"directory share {userName} {item.Path}";

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
                if (result == null)
                    return;
                if (result.StartsWith("_ERROR_"))
                {
                    MessageBox.Show(result);
                }
                else
                {
                    MessageBox.Show("Udostępniono");
                }
            };
        }
        private void  StopShare(object sender, RoutedEventArgs e)
        {
            var item = DirectoryTreeView.SelectedItem as MCStructure;
            var inputDialog = new InputDialog("Podaj nazwę użytkownika");
            string userName = null;
            if (inputDialog.ShowDialog() == true)
                userName = inputDialog.Answer;
            string request = $"directory stop_share {userName} {item.Path}";

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
                if (result == null)
                    return;
                if (result.StartsWith("_ERROR_"))
                {
                    MessageBox.Show(result);
                }
                else
                {
                    MessageBox.Show("Zakończono udostępnianie");
                }
            };
        }
        private void ListUsersWithAccess(object sender, RoutedEventArgs e)
        {
            var item = DirectoryTreeView.SelectedItem as MCStructure;
            string request = $"directory list_users_with_access {item.Path}";
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
                if (result == null)
                    return;
                if (result.StartsWith("_ERROR_"))
                {
                    MessageBox.Show(result);
                }
                else
                {
                   MessageBox.Show(result);
                }
            };
        }
    }
}
