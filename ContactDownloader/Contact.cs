using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GContactSync
{
    /// <summary>
    /// A mock class, mostly for tests
    /// </summary>
    public class Contact : ContactBase
    {
        private string _fullName;
        public override string FullName { get { return _fullName; } set { _fullName = value; } }

        private HashSet<string> EmailList = new HashSet<string>();
        public override IEnumerable<string> Emails { get { return EmailList; } }

        public Contact(string name)
        {
            FullName = name;
        }
        public Contact(string name, string mail)
        {
            FullName = name;
            addMail(mail);
        }
        public Contact(IContact other)
        {
            FullName = other.FullName;
            foreach (string email in other.Emails)
            {
                addMail(email);
            }
        }

        protected override bool internal_addMail(string mail)
        {
            return EmailList.Add(mail);
        }

        public override void Update()
        {
            // noop
        }
    }
}
