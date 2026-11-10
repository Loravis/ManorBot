using ManorBot.Persistence.Daos;
using ManorBot.Persistence.Models;
using Microsoft.Data.Sqlite;

namespace Tests
{
    public class SQLiteManager : IDisposable
    {
        public SqliteConnection Connection { get; }
        public string DbPath { get; }

        public SQLiteManager()
        {
            var dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ManorBot");
            Directory.CreateDirectory(dir);
            DbPath = SQLiteManager.GenPath();

            var builder = new SqliteConnectionStringBuilder
            {
                DataSource = DbPath,
                Pooling = false
            };
            Connection = new SqliteConnection(builder.ToString());
            Connection.Open();
            SetupDatabase();
        }

        /// <summary>
        /// This method is called either by the SQLiteManager constructor
        /// or by the test methods. It should NOT be called by 
        /// test methods that instantiate SQLiteManager, as this would 
        /// result in two database paths being generated, 
        /// which would cause test failures. 
        /// </summary>
        /// <returns></returns>
        public static string GenPath()
        {
            var dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ManorBot");
            Directory.CreateDirectory(dir);
            return Path.Combine(dir, $"test_{Guid.NewGuid():N}.db");
        }

        public void Dispose()
        {
            Connection.Dispose();
            SqliteConnection.ClearAllPools();
            if (File.Exists(DbPath)) File.Delete(DbPath);
        }

        private void SetupDatabase()
        {
            string query = @"
                CREATE TABLE Users (
                    id INTEGER PRIMARY KEY,
                    totxp INTEGER NOT NULL,
                    periodxp INTEGER NOT NULL,
                    level INTEGER NOT NULL
                )";

            using (var command = Connection.CreateCommand())
            {
                command.CommandText = query;
                command.ExecuteNonQuery();
            }

            query = @"
                CREATE TABLE Items (
                    id INTEGER PRIMARY KEY,
                    userId INTEGER NOT NULL,
                    itemId TEXT NOT NULL,
                    count INTEGER NOT NULL,
                    FOREIGN KEY (userId) REFERENCES Users(id) ON DELETE CASCADE
                )";

            using (var command = Connection.CreateCommand())
            {
                command.CommandText = query;
                command.ExecuteNonQuery();
            }

            query = @"
                CREATE INDEX idx_items_userId ON Items(userId)";

            using (var command = Connection.CreateCommand())
            {
                command.CommandText = query;
                command.ExecuteNonQuery();
            }
        }
    }

    [TestClass]
    public sealed class Persistence_SQLiteDaoTests
    {
        [TestMethod]
        public void GetUserById_ReturnNull()
        {
            using SQLiteDao dao = new(SQLiteManager.GenPath());

            // 1 is an appropriate user ID for testing, as this ID is unlikely to exist 
            var ret = dao.GetUserById(1);

            Assert.IsNull(ret);
        }

        [TestMethod]
        public void GetUserById_ReturnCorrectUser()
        {
            using var sql = new SQLiteManager();

            string query = @"
                INSERT INTO Users (id, totxp, periodxp, level)
                VALUES (1, 100, 100, 1)";

            using var command = sql.Connection.CreateCommand();
            command.CommandText = query;
            command.ExecuteNonQuery();

            var dao = new SQLiteDao(sql.DbPath);

            var ret = dao.GetUserById(1);

            Assert.IsNotNull(ret);
            Assert.AreEqual(1, ret.Id);
            Assert.AreEqual(100, ret.TotalXp);
            Assert.AreEqual(100, ret.PeriodXp);
            Assert.AreEqual(1, ret.Level);
        }

        [TestMethod]
        public void AddOrUpdateUser_AddUserCorrectly()
        {
            using var sql = new SQLiteManager();

            var dao = new SQLiteDao(sql.DbPath);

            dao.AddOrUpdateUser(new User(1, 100, 100, 1));

            

            string query = @"SELECT id, totxp, periodxp, level FROM Users WHERE id = 1";
            using var command = sql.Connection.CreateCommand();
            command.CommandText = query;

            using var reader = command.ExecuteReader();

            Assert.IsTrue(reader.Read());
            do
            {
                var id = reader.GetInt64(0);
                var totxp = reader.GetInt64(1);
                var periodxp = reader.GetInt64(2);
                var level = reader.GetInt64(3);

                Assert.AreEqual(1, id);
                Assert.AreEqual(100, totxp);
                Assert.AreEqual(100, periodxp);
                Assert.AreEqual(1, level);
            } while (reader.Read());
        }

        [TestMethod]
        public void AddOrUpdateUser_UpdateUserCorrectly()
        {
            using var sql = new SQLiteManager();
            string query = @"
                INSERT INTO Users (id, totxp, periodxp, level)
                VALUES (1, 100, 100, 1)";
            using var command = sql.Connection.CreateCommand();
            command.CommandText = query;
            command.ExecuteNonQuery();

            var dao = new SQLiteDao(sql.DbPath);
            dao.AddOrUpdateUser(new User(1, 200, 200, 2));

            query = "SELECT id, totxp, periodxp, level FROM Users WHERE id = 1";
            using var command2 = sql.Connection.CreateCommand();
            command2.CommandText = query;

            using var reader = command2.ExecuteReader();
            Assert.IsTrue(reader.Read());
            
            do
            {
                var id = reader.GetInt64(0);
                var totxp = reader.GetInt64(1);
                var periodxp = reader.GetInt64(2);
                var level = reader.GetInt64(3);
                Assert.AreEqual(1, id);
                Assert.AreEqual(200, totxp);
                Assert.AreEqual(200, periodxp);
                Assert.AreEqual(2, level);
            } while (reader.Read());
        }

        [TestMethod]
        public void DeleteUser_DeleteUserCorrectly()
        {
            using var sql = new SQLiteManager();
            string query = @"
                INSERT INTO Users (id, totxp, periodxp, level)
                VALUES (1, 100, 100, 1)";

            using var command = sql.Connection.CreateCommand();
            command.CommandText = query;
            command.ExecuteNonQuery();

            var dao = new SQLiteDao(sql.DbPath);
            var ret = dao.DeleteUser(1);

            Assert.IsTrue(ret);

            query = @"SELECT id, totxp, periodxp, level FROM Users WHERE id = 1";
            using var command2 = sql.Connection.CreateCommand();
            command2.CommandText = query;

            using var reader = command2.ExecuteReader();
            Assert.IsFalse(reader.Read());
        }

        [TestMethod]
        public void DeleteUser_ReturnFalseIfUserDoesNotExist()
        {
            using SQLiteDao dao = new(SQLiteManager.GenPath());
            var ret = dao.DeleteUser(1);
            Assert.IsFalse(ret);
        }

        [TestMethod]
        public void AddItem_AddItemCorrectly()
        {
            using var sql = new SQLiteManager();
            string query = @"
                INSERT INTO Users (id, totxp, periodxp, level)
                VALUES (1, 100, 100, 1)";

            using var command = sql.Connection.CreateCommand();
            command.CommandText = query;
            command.ExecuteNonQuery();

            var dao = new SQLiteDao(sql.DbPath);
            var item = new Item(1, 1, "item1", 1);
            var ret = dao.AddItem(item);

            Assert.IsTrue(ret);

            query = @"SELECT id, userId, itemId, count FROM Items WHERE id = 1";
            using var command2 = sql.Connection.CreateCommand();
            command2.CommandText = query;

            using var reader = command2.ExecuteReader();
            Assert.IsTrue(reader.Read());
            
            do
            {
                var id = reader.GetInt64(0);
                var userId = reader.GetInt64(1);
                var itemId = reader.GetString(2);
                var count = reader.GetInt32(3);
                Assert.AreEqual(1, id);
                Assert.AreEqual(1, userId);
                Assert.AreEqual("item1", itemId);
                Assert.AreEqual(1, count);
            } while(reader.Read());
        }

        [TestMethod]
        public void AddItem_ReturnFalseIfUserDoesNotExist()
        {
            using SQLiteDao dao = new (SQLiteManager.GenPath());
            var item = new Item(1, 1, "item1", 1);
            var ret = dao.AddItem(item);
            Assert.IsFalse(ret);
        }

        [TestMethod]
        public void DeleteItem_DeleteItemCorrectly()
        {
            using var sql = new SQLiteManager();
            string query = @"
                INSERT INTO Users (id, totxp, periodxp, level)
                VALUES (1, 100, 100, 1)";

            using var command = sql.Connection.CreateCommand();
            command.CommandText = query;
            command.ExecuteNonQuery();

            query = @"
                INSERT INTO Items (id, userId, itemId, count)
                VALUES (1, 1, 'item1', 1)";
            using var command2 = sql.Connection.CreateCommand();
            command2.CommandText = query;
            command2.ExecuteNonQuery();

            using SQLiteDao dao = new(sql.DbPath);
            var item = new Item(1, 1, "item1", 1);
            var ret = dao.DeleteItem(item);

            Assert.IsTrue(ret);
            query = @"SELECT id, userId, itemId, count FROM Items WHERE id = 1";
            using var command3 = sql.Connection.CreateCommand();
            command3.CommandText = query;

            using var reader = command3.ExecuteReader();

            Assert.IsFalse(reader.Read());
        }

        [TestMethod]
        public void DeleteItem_ReturnFalseIfItemDoesNotExist()
        {
            using SQLiteDao dao = new(SQLiteManager.GenPath());
            var item = new Item(1, 1, "item1", 1);
            var ret = dao.DeleteItem(item);
            Assert.IsFalse(ret);
        }

        [TestMethod]
        public void DeleteItem_DecrementCountCorrectly()
        {
            using var sql = new SQLiteManager();
            string query = @"
                INSERT INTO Users (id, totxp, periodxp, level)
                VALUES (1, 100, 100, 1)";
            using var command = sql.Connection.CreateCommand();
            command.CommandText = query;
            command.ExecuteNonQuery();

            query = @"
                INSERT INTO Items (id, userId, itemId, count)
                VALUES (1, 1, 'item1', 2)";
            using var command2 = sql.Connection.CreateCommand();
            command2.CommandText = query;
            command2.ExecuteNonQuery();
            using SQLiteDao dao = new(sql.DbPath);
            var item = new Item(1, 1, "item1", 1);
            var ret = dao.DeleteItem(item);

            Assert.IsTrue(ret);
            query = @"SELECT id, userId, itemId, count FROM Items WHERE id = 1";
            using var command3 = sql.Connection.CreateCommand();
            command3.CommandText = query;
            using var reader = command3.ExecuteReader();

            Assert.IsTrue(reader.Read());
            do
            {
                var id = reader.GetInt64(0);
                var userId = reader.GetInt64(1);
                var itemId = reader.GetString(2);
                var count = reader.GetInt32(3);
                Assert.AreEqual(1, id);
                Assert.AreEqual(1, userId);
                Assert.AreEqual("item1", itemId);
                Assert.AreEqual(1, count);
            } while(reader.Read());
        }

        [TestMethod]
        public void GetItems_ReturnsCorrectItems()
        {
            using var sql = new SQLiteManager();
            string query = @"
                INSERT INTO Users (id, totxp, periodxp, level)
                VALUES (1, 100, 100, 1)";

            using var command = sql.Connection.CreateCommand();
            command.CommandText = query;
            command.ExecuteNonQuery();

            query = @"
                INSERT INTO Items (id, userId, itemId, count)
                VALUES (1, 1, 'item1', 2)";

            using var command2 = sql.Connection.CreateCommand();
            command2.CommandText = query;
            command2.ExecuteNonQuery();

            var dao = new SQLiteDao(sql.DbPath);
            var items = dao.GetItems(1);
            Assert.HasCount(1, items);
            Assert.AreEqual(1, items[0].Id);
            Assert.AreEqual(1, items[0].UserId);
            Assert.AreEqual("item1", items[0].ItemId);
            Assert.AreEqual(2, items[0].Count);
        }
    }
}
