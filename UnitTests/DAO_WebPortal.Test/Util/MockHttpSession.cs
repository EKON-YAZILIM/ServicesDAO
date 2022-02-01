using System;
using Xunit;
using FluentAssertions;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Routing.Constraints;
using System.Linq;

namespace DAO_WebPortal.Test.Util
{ 
/// <summary>
/// Mocks HttpContext Session to set session parameters and use in controllers.
/// </summary>
public class MockHttpSession : ISession{
    readonly Dictionary<string, object> _sessionStorage = new Dictionary<string, object>();
    //public string Id => throw new NotImplementedException();

    string ISession.Id=> throw new NotImplementedException();
    bool ISession.IsAvailable=>throw new NotImplementedException();
    IEnumerable<string> ISession.Keys=>_sessionStorage.Keys;
    /// Mock session Set parameter method
    public void Set(string key, byte[] value)
    {
            //Array.Reverse(value);
            var idd = Encoding.UTF8.GetChars(value);
            var idd1 = Encoding.UTF8.GetString(value);
            var t = BitConverter.ToString(value);
        _sessionStorage[key] = Encoding.UTF8.GetString(value);
    }

    /// Mock session Get parameter method.
    public bool TryGetValue(string key, out byte[] value)
    {
        if (_sessionStorage[key] != null){
            value = Encoding.ASCII.GetBytes(_sessionStorage[key].ToString());
            return true;
        }
        value = null;
        return false;
    }
    void ISession.Clear(){
        _sessionStorage.Clear();
    }
    Task ISession.CommitAsync(System.Threading.CancellationToken cancellationToken){
        throw new NotImplementedException();
    }
    Task ISession.LoadAsync(System.Threading.CancellationToken cancellationToken){
        throw new NotImplementedException();
    }
    void ISession.Remove(string key){
        _sessionStorage.Remove(key);
    }
}
}
    