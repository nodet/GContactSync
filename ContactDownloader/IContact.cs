using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GContactSync
{
    public abstract class IContact
    {
        public abstract string FullName { get; set; }
        public abstract IEnumerable<string> Emails {get;}
        public abstract bool addMail(string mail);

        /// <summary>
        /// Returns true if the two contacts should be considered to represent the same entity (and thus be merged)
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public abstract bool IsSameAs(IContact other);

        /// <summary>
        /// Copies the information from one contact to another
        /// </summary>
        /// <param name="other">The source from which the information is copied</param>
        /// <returns>true if some information was actually copied</returns>
        public abstract bool MergeFrom(IContact other);

        /// <summary>
        /// Saves the contact information back to the store
        /// </summary>
        public abstract void Update();
    }

    public abstract class ContactBase : IContact {
        public override bool IsSameAs(IContact other)
        {
            return FullName.Equals(other.FullName) || Emails.Intersect(other.Emails).GetEnumerator().MoveNext();
        }
        public override bool MergeFrom(IContact other)
        {
            bool didSomething = false;
            if (string.IsNullOrEmpty(FullName))
            {
                FullName = other.FullName;
                didSomething = true;
            }
            foreach (string email in other.Emails)
            {
                didSomething |= addMail(email);
            }
            return didSomething;
        }
    }

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

        public override bool addMail(string mail)
        {
            return EmailList.Add(mail);
        }

        public override void Update()
        {
            // noop
        }

    }
}
