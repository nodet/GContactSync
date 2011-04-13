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

    class MockUserCredentialsDialog : IUserCredentials 
    {
        private string _user;
        private string _pass;
        private System.Windows.Forms.DialogResult _result;

        public MockUserCredentialsDialog()
        {
            Init(GoogleContactDownloader.TestUser, GoogleContactDownloader.TestPass, System.Windows.Forms.DialogResult.OK);
        }

        #region IUserCredentials Members

        public string User
        {
            get { return _user; }
        }

        public string Pass
        {
            get { return _pass; }
        }

        System.Windows.Forms.DialogResult IUserCredentials.ShowDialog()
        {
            return _result;
        }

        #endregion

        private void Init(string user, string pass, System.Windows.Forms.DialogResult result)
        {
	        _user = user;
            _pass = pass;
            _result = result;
        }
    }

    /// <summary>
    /// Summary description for UserCredentialsAskerTests
    /// </summary>
    [TestClass]
    public class UserCredentialsAskerTests
    {
        public UserCredentialsAskerTests()
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
        public void TestGetUserCredentialsReturnsCorrectInformation()
        {
            IUserCredentials uc = new MockUserCredentialsDialog();
            string user;
            string pass;
            UserCredentialsAsker.GetUserCredentials(uc, out user, out pass);
            Assert.IsFalse(string.IsNullOrEmpty(user));
            Assert.IsFalse(string.IsNullOrEmpty(pass));
        }

    }



}
