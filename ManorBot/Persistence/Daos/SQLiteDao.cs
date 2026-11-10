using ManorBot.Persistence.Models;

namespace ManorBot.Persistence.Daos
{
    public class SQLiteDao : IDao
    {
        private readonly string DbPath;

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public SQLiteDao(string dbPath)
        {
            DbPath = dbPath;
            throw new NotImplementedException();
        }

        public User? GetUserById(long userId)
        {
            throw new NotImplementedException();
        }

        public void AddOrUpdateUser(User user)
        {
            throw new NotImplementedException();
        }

        public bool DeleteUser(long userId)
        {
            throw new NotImplementedException();
        }

        public bool AddItem(Item item)
        {
            throw new NotImplementedException();
        }

        public bool DeleteItem(Item item)
        {
            throw new NotImplementedException();
        }

        public List<Item> GetItems(long userId)
        {
            throw new NotImplementedException();
        }
    }
}
