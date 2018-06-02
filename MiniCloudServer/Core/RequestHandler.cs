using MiniCloudServer.Exceptions;
using MiniCloudServer.Persistence;
using MultiServer.Services;
using Server.Services;
using System;
using System.Linq;
using System.Text;

namespace MiniCloud.Core
{
    class RequestHandler
    {
        private readonly Connection _currentClient;
        private readonly AccountService _accountService;

        public RequestHandler(Connection currentClientConnection)
        {
            _currentClient = currentClientConnection;
            var dbContext=new MiniCloudContext();
            var encryptService=new EncryptService();
            _accountService=new AccountService(dbContext,encryptService,currentClientConnection);
        }

        public void Handle(string request)
        {
            var command=request.Split().First();
            switch(command)
            {
                //account register [userName] [password]
                case "register":
                    RegisterUser(request);
                    break;
                case "login":
                    LoginUser(request);
                    break;
                case "say_my_name":
                    SayMyName();
                    break;
                default:
                    InvalidRequest();
                    break;
            }
            
        }

        private void SayMyName()
        {
            try
            {
                var user = _accountService.GetLoggedUser();
                _currentClient.SendText($"You are {user.UserName}");
            }
            catch (MiniCloudException ex)
            {
                _currentClient.SendText(ex.Message);
            }
        }

        private void LoginUser(string request)
        {
            string[] arguments = request.Split();
            if (arguments.Length < 3 || arguments.Any(x=>String.IsNullOrWhiteSpace(x)))
            {
                _currentClient.SendText("Usage: login <user_name> <password>");
                return;
            }
            string userName = arguments[1];
            string password = arguments[2];
            try
            {
                _accountService.LoginUser(userName,password);
                _currentClient.SendText($"Welcome");
            }
            catch (MiniCloudException ex)
            {
                _currentClient.SendText(ex.Message);
            }
        }

        private void RegisterUser(string request)
        {
            string[] arguments = request.Split();
            if (arguments.Length < 3 || arguments.Any(x => String.IsNullOrWhiteSpace(x)))
            {
                _currentClient.SendText("Usage: register <user_name> <password>");
                return;
            }
            string userName=arguments[1];
            string password=arguments[2];
            try {
                _accountService.RegisterUser(userName,password);
                _currentClient.SendText("Created.");
            }
            catch(MiniCloudException ex)
            {
                _currentClient.SendText(ex.Message);
            }

            
        }

        private void InvalidRequest()
        {
            _currentClient.SendText("Invalid request");
        }

        //private void ListClients()
        //{
        //    var stringBuilder=new StringBuilder();
        //    int number=0;
        //    foreach(var client in Program.connections)
        //    {
        //        stringBuilder.Append($"{number++}. ");
        //        if(client.FirstName==null)
        //            stringBuilder.Append("Unnamed");
        //        else
        //            stringBuilder.Append($"{client.FirstName} {client.LastName}");
        //        stringBuilder.AppendLine();
        //    }
        //    _currentClient.SendText(stringBuilder.ToString());
        //}

        //private void SetName(string request)
        //{
        //    string[] arguments = request.Split();
        //    if (arguments.Length != 3)
        //    {
        //        _currentClient.SendText("Usage: set_name <first_name> <last_name>");
        //    }
        //    else
        //    {
        //        string firstName = arguments[1];
        //        string lastName = arguments[2];
        //        _currentClient.FirstName = firstName;
        //        _currentClient.LastName = lastName;
        //        _currentClient.SendText($"Hello {firstName} {lastName}");
        //    }
        //}
    }
}
