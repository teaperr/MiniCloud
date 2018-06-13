using MiniCloudServer.Core;

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
