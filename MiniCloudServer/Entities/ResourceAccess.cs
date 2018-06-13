using MiniCloudServer.Entities;
using System;

namespace MiniCloudServer.Entities
{
    public class ResourceAccess
    {
        public int Id { get; private set;}
        public string Path { get; private set; }
        public User OwnerUser { get; private set; }
        public User DoneeUser { get; private set; }
        public int OwnerUserId { get; private set;}
        public int DoneeUserId { get; private set; }


        public ResourceAccess(string path, int ownerUserId, int doneeUserId)
        {
            Path = path;
            OwnerUserId=ownerUserId;
            DoneeUserId=doneeUserId;
        }
        private ResourceAccess()
        {

        }
    }
}
