using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GContactSync
{
    public interface IContact
    {
        string FullName { get; set; }
        IEnumerable<string> Emails {get;}

        /// <summary>
        /// Adds an email address to the contact.
        /// </summary>
        /// <param name="mail"></param>
        /// <returns>true if the address was not already in the contact information.</returns>
        bool addMail(string mail);

        /// <summary>
        /// Returns true if the two contacts should be considered to represent the same entity (and thus be merged)
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        bool IsSameAs(IContact other);

        /// <summary>
        /// Copies the information from one contact to another
        /// </summary>
        /// <param name="other">The source from which the information is copied</param>
        /// <returns>true if some information was actually copied</returns>
        bool MergeFrom(IContact other);

        /// <summary>
        /// Returns true if and only if the contact contains at least a name or an email address
        /// </summary>
        /// <returns></returns>
        bool ContainsSomeInformation();

        /// <summary>
        /// Saves the contact information back to the store
        /// </summary>
        void Update();
    }
}
