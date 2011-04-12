using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace GContactSync
{
    public interface IContactManager
    {
        IEnumerable<IContact> GetContacts();
        IContact NewContact(IContact other);
    }

    public class MockContactManager : IContactManager 
    {
        public Func<String, String, Boolean> AuthenticateImpl { get; set; }
        public Func<List<IContact>> GetContactsImpl { get; set; }
         
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

        public IContact NewContact(IContact other)
        {
            IContact newC = new Contact(other);
            GetContactsImpl().Add(newC);
            return newC;
        }

    }
}
