using MiniCloudServer.Core;

namespace MiniCloudServer.Controllers
{
    public abstract class Controller: IController
    {
        protected Session Session;

        public void SetSession(Session session)
        {
            Session=session;
        }
    }
}
