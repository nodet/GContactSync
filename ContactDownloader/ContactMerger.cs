using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace GContactSync
{
    public class ContactMerger
    {
        public static void Merge(IContactManager m1, IContactManager m2)
        {
            foreach (IContact c in m1.GetContacts())
            {
                foreach (IContact oc in m2.GetContacts())
                {
                    if (!c.IsSameAs(oc)) continue;
                    c.MergeFrom(oc);
                    oc.MergeFrom(c);
                }
            }
        }
    }
}
