using Autofac;
using MiniCloudServer.Controllers;
using MiniCloudServer.Exceptions;
using MiniCloudServer.Extensions;
using MiniCloudServer.IoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MiniCloudServer.Core
{
    public class RequestHandler
    {
        private readonly Connection _currentClient;

        public RequestHandler(Connection currentClientConnection)
        {
            _currentClient = currentClientConnection;
        }

        public async Task Handle(string request)
        {
            try {
                var splitedRequest= request.Split();
                if(splitedRequest.Length<2)
                    throw new MiniCloudException("Invalid Request");
                var controllerName= splitedRequest[0];
                var controllerType=GetControllerType(controllerName);
                if(controllerType==null)
                    throw new MiniCloudException($"Controller {controllerName} doesn't exists");

                var methodName= splitedRequest[1];
                var methodInfo=GetMethod(controllerType,methodName);
                if (methodInfo == null)
                    throw new MiniCloudException($"Method {methodName} doesn't exists in {controllerName} controller");

                var parameters=methodInfo.GetParameters();
                var arguments=splitedRequest.Skip(2);
                if(arguments.Count()!=parameters.Count() || arguments.Any(x=>String.IsNullOrWhiteSpace(x)))
                {
                    var correctUsage=GenerateCorrectUsageInfo(controllerName, methodName, parameters.Select(x => x.Name));
                    throw new MiniCloudException(correctUsage);
                }
                    
                //var controller=Activator.CreateInstance(controllerType,_currentClient.Session);
                var controller=Bootstraper.Container.ResolveNamed<IController>(controllerType.Name);
                controller.SetSession(_currentClient.Session);

                string response;
                if(methodInfo.ReturnType==typeof(Task<string>))
                {
                    var task= (Task<string>)methodInfo.Invoke(controller, arguments.ToArray());
                    await task;
                    response=task.Result;
                }
                else
                    throw new Exception("Wrong return type");

                _currentClient.SendText($"_OK_: {response}");
            }
            catch(MiniCloudException ex)
            {
                _currentClient.SendText($"_ERROR_: {ex.Message}");
            }
            catch (Exception ex)
            {
                _currentClient.SendText("_ERROR_: Internal Error");
            }
        }



        private Type GetControllerType(string name)
        {
            var asm= typeof(RequestHandler).Assembly;
            var controller=asm.GetTypes()
                .Where(x=>typeof(IController).IsAssignableFrom(x))
                .Where(x=>String.Compare(NormalizeControllerName(x.Name),name)==0)
                .SingleOrDefault();
            return controller;
        }

        private string NormalizeControllerName(string controllerName)
        {
            return controllerName.Replace("Controller", "", StringComparison.InvariantCultureIgnoreCase).ToUnderScore();
        }

        private MethodInfo GetMethod(Type controllerType, string methodName)
        {
            return controllerType.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(x=> String.Compare(x.Name.ToUnderScore(),methodName)==0)
                .SingleOrDefault();
        }

        private string GenerateCorrectUsageInfo(string controllerName, string methodName, IEnumerable<string> parameters)
        {
            //"Usage: account login_user <user_name> <password>"
            var stringBuilder=new StringBuilder();
            stringBuilder.Append("Usage: ")
                .Append(controllerName).AppendSpace()
                .Append(methodName).AppendSpace();
            foreach(var parameter in parameters)
            {
                stringBuilder.Append("<")
                    .Append(parameter.ToUnderScore())
                    .Append(">")
                    .AppendSpace();
            }
            return stringBuilder.ToString();

        }
    }
}
