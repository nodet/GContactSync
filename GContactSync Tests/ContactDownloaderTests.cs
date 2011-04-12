using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GContactSync;

using Google.GData.Client;
using Google.GData.Extensions;
using Google.GData.Contacts;
using Google.Contacts;

namespace GContactSync_Tests
{
    /// <summary>
    /// Summary description for ContactDownloaderTests
    /// </summary>
    [TestClass]
    public class ContactDownloaderTests
    {
        public ContactDownloaderTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestGetEmptyListOfContacts()
        {
            IContactManager cd = new MockContactManager
            {
                GetContactsImpl = () => { return new List<IContact>(); }

            };
            IEnumerable<IContact> contacts = cd.GetContacts();
            Assert.IsFalse(contacts.GetEnumerator().MoveNext());
        }


        [TestMethod]
        public void TestGetListOfOneContact()
        {
            IContactManager cd = new MockContactManager
            {
                GetContactsImpl = () => { 
                    var l = new List<IContact>();
                    l.Add(new GContactSync.Contact("John Doe"));
                    return l;
                }

            };
            IEnumerable<IContact> contacts = cd.GetContacts();
            var it = contacts.GetEnumerator();
            Assert.IsTrue(it.MoveNext());
            Assert.IsTrue(it.Current.FullName.Equals("John Doe"));
        }


        //[TestMethod]
        public void TestGetRealListOfContacts()
        {
            IContactManager cd = new GoogleContactDownloader("xavier.nodet@gmail.com", "");
            //IContactDownloader cd = new MockContactDownloader
            //{
            //    GetContactsImpl = () =>
            //    {
            //        var l = new List<IContact>();
            //        l.Add(new Contact("John Doe"));
            //        return l;
            //    }

            //};

            int nbContact = 0;
            int nbMails = 0;
            foreach (IContact c in cd.GetContacts()) {
                nbContact++;
                foreach (string m in c.Emails) {
                    nbMails++;
                }
            }
            System.Windows.Forms.MessageBox.Show("You have " + nbContact + " contacts totalling " + nbMails + " email addresses.");
        }


        //[TestMethod]
        public void TestCreateAGoogleContact()
        {
            GContactSync.Contact c = new GContactSync.Contact("John Doe");
            //c.addMail("john@doe.com");

            GoogleContactDownloader gcd = new GoogleContactDownloader("xavier.nodet@gmail.com", "");
            IContact gc = gcd.NewContact(c);
            gc.Update();
        }

        //[TestMethod]
        public void TestTheGoogleWay()
        {
            try {
                RequestSettings rs = new RequestSettings("GContactSync", "xavier.nodet@gmail.com", "");
                ContactsRequest cr = new ContactsRequest(rs);

                Google.Contacts.Contact entry = new Google.Contacts.Contact();
                entry.Name = new Name();
                entry.Name.FullName = "John Doe";
                entry.Emails.Add(new EMail("john@doe.com", ContactsRelationships.IsOther));
                Uri feedUri = new Uri(ContactsQuery.CreateContactsUri("default"));
                cr.Insert(feedUri, entry);
            } 
            //catch (GDataRequestException ex) {
            //    throw ex;
            //}
            catch  (System.Exception ex) {
                throw ex;
            }
        }

        //[TestMethod]
        public void testMergeWithGoogle()
        {
            try
            {
                GoogleContactDownloader gcd = new GoogleContactDownloader("xavier.nodet@gmail.com", "");

                IContact c1 = new GContactSync.Contact("John Doe", "john@doe.com");
                IContactManager m1 = new MockContactManager
                {
                    GetContactsImpl = () =>
                    {
                        var l = new List<IContact>();
                        l.Add(c1);
                        return l;
                    }

                };
                IEnumerable<IContact> googleContacts = gcd.GetContacts();
                googleContacts = googleContacts.Where(c => c.FullName != null && c.FullName.Contains("Doe"));

                ContactMerger.Merge(gcd, m1, googleContacts, m1.GetContacts());
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

        }

    }



}
