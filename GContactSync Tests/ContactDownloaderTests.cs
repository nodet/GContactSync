using System;
using System.Collections.Generic;
using System.Linq;
using GContactSync;
using Google.Contacts;
using Google.GData.Client;
using Google.GData.Contacts;
using Google.GData.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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


        [TestMethod]
        public void TestCreateAGoogleContact()
        {
            GContactSync.Contact c = new GContactSync.Contact("John Doe");
            c.addMail("john@doe.com");

            GoogleContactDownloader gcd = new GoogleContactDownloader(GoogleContactDownloader.TestUser, GoogleContactDownloader.TestPass);
            int oldCount = gcd.GetContacts().Count();

            IContact gc = gcd.NewContact(c);
            gc.Update();

            Assert.AreEqual(gcd.GetContacts().Count(), oldCount + 1);
        }

        [TestMethod]
        public void TestTheGoogleWay()
        {
            try {
                RequestSettings rs = new RequestSettings("GContactSync", GoogleContactDownloader.TestUser, GoogleContactDownloader.TestPass);
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

        [TestMethod]
        public void testMergeWithGoogle()
        {
            try
            {
                GoogleContactDownloader gcd = new GoogleContactDownloader(GoogleContactDownloader.TestUser, GoogleContactDownloader.TestPass);

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
