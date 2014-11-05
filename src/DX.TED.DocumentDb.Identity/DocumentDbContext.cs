using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration;
using System.Diagnostics;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Documents;



namespace DX.TED.DocumentDb.Identity
{
    public class DocumentDbContext : IDisposable
    {

        string _connectionString;
        Uri _uri;
        string _databaseName;
        string _authKey;
        bool _autoInit = false;
        bool _clientInitialized = false;
        Database _docDatabase;
        DocumentClient _client = null;
        DocumentCollection _userCollection = null;

        const string _collectionName = "IdentityUsers";

        public string AuthKey
        {
            get { return _authKey; }
            private set { _authKey = value; }
        }
        public Uri Uri
        {
            get { return _uri; }
            private set { _uri = value; }
        }
        public Database Database
        {
            get { return _docDatabase; }
            private set { _docDatabase = value; }
        }

        public string CollectionName
        {
            get { return _collectionName; }
        }
        public DocumentCollection UserCollection
        {
            get { return _userCollection; }
            set { _userCollection = value; }
        }
        public DocumentClient Client
        {
            get
            {
                if (!_clientInitialized)
                {
                    _clientInitialized = Initialize();;
                    if (!_clientInitialized)
                        Debug.WriteLine("didn't wait for initialization");
                }

                return GetClient();
            }
        }

        DocumentClient GetClient()
        {
            if (null == _client)
                _client = new DocumentClient(_uri, _authKey);

            return _client;
        }

        /// Constructors
        public DocumentDbContext()
            : this("DefaultDocDbConnection")
        {
        }

        public DocumentDbContext(string connectionElement)
            : this(connectionElement, false)
        {
        }

        public DocumentDbContext(string connectionElement, bool lazyInitialize = false)
        {
            ValidateConfiguration(connectionElement);
            ParseConnectionString();

            if (!lazyInitialize)
                Initialize();//.Start();//.RunSynchronously();//.Wait();
        }

        /// <summary>
        /// Does a quick validate of the connection and fails fast...
        /// </summary>
        bool Initialize()
        {
            /// TODO:worry about policy and consistency level later..
            RunInitAsync();//.Wait();
            _clientInitialized = true;

            if (null != _docDatabase)
            {
                Trace.Assert(!string.IsNullOrEmpty(_docDatabase.SelfLink));
            }

            return _clientInitialized;
        }

        void RunInitAsync()
        {
            var client = GetClient();
            bool databaseExists = false;
            var mainTask = client.OpenAsync().ContinueWith((t1) =>
            {
                if (t1.IsFaulted)
                    throw t1.Exception;

                this.Database = client.CreateDatabaseQuery()
                    .Where(db => db.Id == _databaseName)
                    .AsEnumerable().FirstOrDefault();

                if (null != this.Database)
                    databaseExists = true;

            }).ContinueWith((t2) =>
            {
                if (t2.IsFaulted)
                    throw t2.Exception;

                if (_autoInit && !databaseExists)
                {
                    this.Database = client.CreateDatabaseAsync(new Database { Id = _databaseName }).Result;
                }

            }).ContinueWith((t3) =>
            {
                if (t3.IsFaulted)
                    throw t3.Exception;

                if (this.Database != null)
                {
                    this.UserCollection = client.GetOrCreateCollectionAsync(this.Database.UsersLink, this.CollectionName).Result;
                    Trace.Assert(this.UserCollection != null, "User Collection is null");
                }
                else
                {
                    throw new InvalidOperationException("Can't get to collection as DB is still null");
                }
            });


            //TODO:magic number
            bool rv = mainTask.Wait(5000);

            if (!rv)
                throw new ApplicationException("stopped waiting ");
        }


        /// <summary>
        /// Parse the connection string the best I can here...
        /// </summary>
        void ParseConnectionString()
        {
            try
            {
                string[] parts = _connectionString.Split(';');
                if (parts.Length < 3)
                    throw new ConfigurationErrorsException("Invalid format for the connection string too few settings: {0}".FormatMessage(_connectionString));


                foreach (var setting in parts)
                {
                    int separator = setting.IndexOf('=');
                    var currentKey = setting.Substring(0, separator).ToLowerInvariant();
                    var currentValue = setting.Substring(separator + 1);
                    if (string.IsNullOrEmpty(currentKey) || string.IsNullOrEmpty(currentValue))
                        throw new ConfigurationErrorsException("Invalid format for the connection string setting: {0} - {1}".FormatMessage(_connectionString, setting));

                    switch (currentKey)
                    {
                        case "uri":
                            _uri = new Uri(currentValue);
                            break;
                        case "authkey":
                            _authKey = currentValue;
                            break;
                        case "database":
                            _databaseName = currentValue;
                            break;
                        case "autoinit":
                            if (currentValue == bool.FalseString.ToLowerInvariant() || currentValue == bool.TrueString.ToLowerInvariant())
                                bool.TryParse(currentValue, out _autoInit);
                            else
                                throw new FormatException("value provided for AutoInit format wrong".FormatMessage(currentValue));
                            break;
                        default:
                            throw new InvalidProgramException("odd attribute in the connection string: {0}".FormatMessage(setting));
                    }
                }
            }
            catch (UriFormatException ex)
            {
                throw new UriFormatException("Uri from connectionString is invalid -- {0}".FormatMessage(ex.Message));
            }

        }

        /// <summary>
        /// Validates configuration element and passed string
        /// </summary>
        /// <param name="connectionElement"></param>
        void ValidateConfiguration(string connectionElement)
        {
            if (null == connectionElement)
                throw new ArgumentNullException("connectionString",
                    Identity.Resources.IdentityResources.ValueCannotBeNullOrEmpty.FormatMessage("connectionString"));

            if (string.IsNullOrEmpty(connectionElement))
                throw new ArgumentException("connectionString",
                    Identity.Resources.IdentityResources.ValueCannotBeNullOrEmpty.FormatMessage("connectionString"));

            var conStringCollection = ConfigurationManager.ConnectionStrings[connectionElement];

            if (null == conStringCollection)
                throw new ConfigurationErrorsException(
                    Identity.Resources.IdentityResources.ConnectionKeyNotFound.FormatMessage(connectionElement));

            var conString = conStringCollection.ConnectionString;

            if (string.IsNullOrEmpty(conString))
                throw new ConfigurationErrorsException(
                    Identity.Resources.IdentityResources.ConnectionStringNotFound.FormatMessage(connectionElement));


            _connectionString = conString;

        }



        public void Dispose()
        {
            //TODO:Proper dispose
            if (_client != null)
                _client.Dispose();
        }
    }
}
