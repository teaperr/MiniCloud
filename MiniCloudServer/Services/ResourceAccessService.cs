using Microsoft.EntityFrameworkCore;
using MiniCloudServer.Core;
using MiniCloudServer.Entities;
using MiniCloudServer.Exceptions;
using MiniCloudServer.Persistence;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCloudServer.Services
{
    public class ResourceAccessService : IResourceAccessService
    {
        private readonly MiniCloudContext _dbContext;
        public ResourceAccessService()
        {
            _dbContext=new MiniCloudContext();
        }
        public async Task ShareAccessToResourceAsync(string doneeName, string ownerName, string path)
        {
            var userPath=PathUtilities.GenerateUserPath(ownerName,path);
            var fullPath=PathUtilities.GenerateFullPath(ownerName,path);
            if(!Directory.Exists(fullPath))
                throw new MiniCloudException("Path is not valid");

            var donneeUser=await _dbContext.Users.Include(x=>x.ResourceAccesses).SingleOrDefaultAsync(x=>x.UserName==doneeName);
            var ownerUser=await _dbContext.Users.SingleOrDefaultAsync(x=>x.UserName==ownerName);
            if(donneeUser==null || ownerUser==null)
                throw new MiniCloudException("User doesn't exists");

            //jeśli przyznano już dostęp do katalogu nadrzędnego to przyznawanie dostępu do aktualnego katalogu nie ma sensu
            if(donneeUser.ResourceAccesses.Any(x=>userPath.Contains(x.Path)))
                throw new Exception("Access alrady granted");

            //jeśli są jakieś dostępy do plików lub katalogów wewnątrz katalogu, którego chcemy dać dostęp to możemy je wyrzucić,
            //bo i tak będzie dostęp z poziomu dodawanego katalogu
            donneeUser.ResourceAccesses.RemoveWhere(x=>x.Path.StartsWith(userPath));  
            
            var resourceAccess=new ResourceAccess(userPath,ownerUser.Id,donneeUser.Id);
            await _dbContext.ResourceAccesses.AddAsync(resourceAccess);
            await _dbContext.SaveChangesAsync();
        }

    }
}
