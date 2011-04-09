using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace GContactSync
{
    public interface IContactManager
    {
        bool Authenticate(string user, string pass);
        IEnumerable<IContact> GetContacts();
    }

    public class MockContactManager : IContactManager 
    {
        public Func<String, String, Boolean> AuthenticateImpl { get; set; }
        public Func<IEnumerable<IContact>> GetContactsImpl { get; set; }
         
        public Boolean Authenticate(string user, string pass) 
        {
            if (AuthenticateImpl == null)
            {
                throw new InvalidOperationException(
                    "Cannot call Authenticate when AuthenticateImpl is null");
            }

            return AuthenticateImpl(user, pass);
        }

        public IEnumerable<IContact> GetContacts()
        {
            if (GetContactsImpl == null)
            {
                throw new InvalidOperationException(
                    "Cannot call GetContacts when GetContactsImpl is null");
            }

            return GetContactsImpl();
        }

    }
}
