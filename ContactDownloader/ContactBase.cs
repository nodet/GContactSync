using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GContactSync
{
    public abstract class ContactBase : IContact
    {
        public override string ToString() {
            if (string.IsNullOrEmpty(FullName))
            {
                if (Emails.Count() > 0)
                {
                    return Emails.ElementAt(0);
                }
                else
                {
                    return "[Empty contact]";
                }
            } 
            else
            {
                return FullName;
            }
        }

        // The API that this class does not implement itself
        public abstract string FullName { get; set; }
        public abstract IEnumerable<string> Emails { get; }
        public abstract void Update();

        /// <summary>
        /// This method must be implemented in subclasses to actually store an email address.
        /// </summary>
        /// <param name="mail">The address to store. Will be neither null, nor empty string.</param>
        /// <returns>true if the address was not already present.</returns>
        protected abstract bool internal_addMail(string mail);

        public bool addMail(string mail)
        {
            if (string.IsNullOrEmpty(mail))
            {
                return false;
            }
            return internal_addMail(mail);
        }
        public bool IsSameAs(IContact other)
        {
            return (FullName != null && FullName.Equals(other.FullName)) || Emails.Intersect(other.Emails).GetEnumerator().MoveNext();
        }
        public bool MergeFrom(IContact other)
        {
            bool didSomething = false;
            if (string.IsNullOrEmpty(FullName) && !string.IsNullOrEmpty(other.FullName))
            {
                FullName = other.FullName;
                didSomething = true;
            }
            if (other.Emails != null)
            {
                foreach (string email in other.Emails)
                {
                    didSomething |= addMail(email);
                }
            }
            return didSomething;
        }
        public bool ContainsSomeInformation()
        {
            return !string.IsNullOrEmpty(FullName) || (Emails.Count() > 0);
        }
    }
}
