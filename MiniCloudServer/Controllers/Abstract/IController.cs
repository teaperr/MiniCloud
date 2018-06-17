using MiniCloudServer.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiniCloudServer.Controllers
{
    interface IController
    {
        void SetSession(Session session);
    }
}
