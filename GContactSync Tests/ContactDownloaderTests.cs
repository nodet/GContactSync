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
        public void TestAuthenticateCanSucceed()
        {
            IContactManager cd = new MockContactManager
            {
                AuthenticateImpl = (u, p) => true
            };
            Assert.IsTrue(cd.Authenticate("john@doe.com", "password"));
        }


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
            IContactManager cd = new GoogleContactDownloader();
            cd.Authenticate("xavier.nodet@gmail.com", "");
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
            RequestSettings rs = new RequestSettings("GContactSync", "xavier.nodet@gmail.com", "");
            GContactSync.Contact c = new GContactSync.Contact("John Doe");
            GContact gc = new GContact(rs, c);
            gc.Update();
        }


    }



}
