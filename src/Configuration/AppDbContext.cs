using api_camem.src.Models;
using MongoDB.Driver;

namespace api_camem.src.Configuration
{
    public class AppDbContext
    {
        public static string? ConnectionString { get; set; }
        public static string? DatabaseName { get; set; }
        public static bool IsSSL { get; set; }
        private IMongoDatabase Database { get; }

        public AppDbContext()
        {
            try
            {
                MongoClientSettings mongoClientSettings = MongoClientSettings.FromUrl(new MongoUrl(ConnectionString));
                if (IsSSL)
                {
                    mongoClientSettings.SslSettings = new SslSettings
                    {
                        EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12
                    };
                }
                
                var mongoClient = new MongoClient(mongoClientSettings);
                Database = mongoClient.GetDatabase(DatabaseName);
            }
            catch(Exception ex)
            {
                throw new Exception($"Failed to connect to database. Error: {ex.Message}");
            }
        }

        #region MASTER DATA
        public IMongoCollection<User> Users => Database.GetCollection<User>("users");
        public IMongoCollection<ProfileUser> ProfileUsers => Database.GetCollection<ProfileUser>("profile_users");
        public IMongoCollection<Customer> Customers => Database.GetCollection<Customer>("customers");
        public IMongoCollection<Supplier> Suppliers => Database.GetCollection<Supplier>("suppliers");
        #endregion

        #region FINANCIAL
        public IMongoCollection<PaymentMethod> PaymentMethods => Database.GetCollection<PaymentMethod>("payment_methods");
        public IMongoCollection<AccountReceivable> AccountsReceivable => Database.GetCollection<AccountReceivable>("accounts_receivable");
        public IMongoCollection<AccountPayable> AccountsPayable => Database.GetCollection<AccountPayable>("accounts_payable"); 
        public IMongoCollection<ChartOfAccounts> ChartOfAccounts => Database.GetCollection<ChartOfAccounts>("chart_of_accounts");
        #endregion

        public IMongoCollection<ServiceOrder> ServiceOrders => Database.GetCollection<ServiceOrder>("service_orders");
        public IMongoCollection<ServiceOrderItem> ServiceOrderItems => Database.GetCollection<ServiceOrderItem>("service_order_items");
        public IMongoCollection<Situation> Situations => Database.GetCollection<Situation>("situations");

        #region SETTINGS
        public IMongoCollection<Logger> Loggers => Database.GetCollection<Logger>("loggers");
        public IMongoCollection<Count> Counts => Database.GetCollection<Count>("counts");
        public IMongoCollection<Template> Templates => Database.GetCollection<Template>("templates");
        public IMongoCollection<Trigger> Triggers => Database.GetCollection<Trigger>("triggers");
        public IMongoCollection<Notification> Notifications => Database.GetCollection<Notification>("notifications");
        #endregion
        
        #region CHAT
        public IMongoCollection<ChatMessage>  ChatMessages  => Database.GetCollection<ChatMessage>("chat_messages");
        public IMongoCollection<Conversation> Conversations => Database.GetCollection<Conversation>("conversations");
        #endregion
    }
}

