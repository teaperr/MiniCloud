using MiniCloudServer.Core;
using MiniCloudServer.Persistence;
using MiniCloudServer.Services;
using MultiServer.Services;
using Server.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MiniCloudServer.Controllers
{
    public class AccessController : Controller
    {
        public AccessController(Session session) : base(session)
        {

        }
    }
}
