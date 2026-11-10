using ManorBot.Persistence.Models;

namespace ManorBot.Persistence.Daos
{
    public interface IDao : IDisposable
    {

        /// <summary>
        /// Get a user from the database by their Discord user ID.
        /// </summary>
        /// <param name="userId">discord user ID.</param>
        /// <returns>
        /// A User object if the user exists in the database, 
        /// or null if the user does not exist.
        /// </returns>
        public User? GetUserById(long userId);

        /// <summary>
        /// Add a user to the database, if they do not already exist, 
        /// and update their information if they do exist.
        /// </summary>
        /// <param name="user"></param>
        public void AddOrUpdateUser(User user);

        /// <summary>
        /// Delete an existing user from the database. 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>
        /// True if the operation was successful, 
        /// false if the user does not exist in the database. 
        /// </returns>
        public bool DeleteUser(long userId);

        /// <summary>
        /// Add an item to the database. 
        /// If the item already exists for the user, 
        /// the count of the item will be incremented 
        /// by the count of the new item.
        /// </summary>
        /// <returns>
        /// True if the operation was successful,
        /// False if the user does not exist in the database.
        /// </returns>
        /// <param name="item"></param>
        public bool AddItem(Item item);

        /// <summary>
        /// Delete an item from the database. 
        /// If multiple copies of the item exist,
        /// the count will be decremented by the 
        /// count of the item to be deleted. 
        /// Internally, if the count of the item reaches 0,
        /// it is removed from the database table. 
        /// </summary>
        /// <param name="item"></param>
        /// <returns>
        /// True if the operation was successful,
        /// False if the item does not exist in the database. 
        /// </returns>
        public bool DeleteItem(Item item);

        /// <summary>
        /// Get a list of all items a user owns. 
        /// 
        /// The caller is expected to ensure that 
        /// the user exists in the database before 
        /// calling this method. An exception will be 
        /// thrown if the user does not exist.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>
        /// A List of Items. If the user has no items
        /// in their inventory, the list will be empty.
        /// </returns>
        public List<Item> GetItems(long userId);
    }
}
