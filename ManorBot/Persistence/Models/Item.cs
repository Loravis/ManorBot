namespace ManorBot.Persistence.Models
{
    /** 
     * <summary>
     * Represents an item that is owned by a bot user.
     * Each user may own an undefined number of items. 
     * 
     * This class is a child of the User class. 
     * </summary>
     */
    internal class Item
    {
        /// <summary>
        /// This property represents the unique primary key
        /// of the item in the database. 
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Represents the user that owns this item. 
        /// Within the database, this property acts 
        /// as the foreign key. 
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// This is a human-readable, unique identifier 
        /// of the item. It is defined within a configuration file. 
        /// </summary>
        public required string ItemId { get; set; }

        /// <summary>
        /// Represents the number of copies a particular user 
        /// has of this specific item. This property exists 
        /// because it would be wasteful to fill the database 
        /// with multiple exact duplicates. 
        /// </summary>
        public int Count
        {
            get;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Count must be at least 0.", nameof(value));
                }
                field = value;
            }
        }
    }
}
