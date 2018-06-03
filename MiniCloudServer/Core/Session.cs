using System;
using System.Collections.Generic;
using System.Text;

namespace MiniCloudServer.Core
{
    public class Session
    {
        private readonly Dictionary<string, object> sessionDict;
        public Session()
        {
            sessionDict=new Dictionary<string, object>();
        }
        public void AddObject(string key, object value)
        {
            sessionDict.Remove(key);
            sessionDict.Add(key,value);
        }
        public object GetObject(string key)
        {
            return sessionDict[key];
        }
        public bool TryGetValue(string key, out object value)
        {
            return sessionDict.TryGetValue(key,out value);
        }
    }
}
