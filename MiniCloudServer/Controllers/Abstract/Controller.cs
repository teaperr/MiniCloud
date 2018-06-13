using MiniCloud.Core;
using MiniCloudServer.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiniCloudServer.Controllers
{
    public class Controller: IController
    {
        protected readonly Session Session;

        public Controller(Session session)
        {
            Session = session;
        }
    }
}
