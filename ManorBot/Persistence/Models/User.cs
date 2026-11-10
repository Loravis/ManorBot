namespace ManorBot.Persistence.Models
{
    /** 
     * <summary>
     * Represents a bot user.
     * </summary>
     */
    public class User
    {
        public User(long id, long totalXp, long periodXp, int level)
        {
            Id = id;
            TotalXp = totalXp;
            PeriodXp = periodXp;
            Level = level;
        }

        /// <summary>
        /// Represents the user's user ID, 
        /// as provided by the Discord API. 
        /// The ID is used as the primary key 
        /// for the user in the database 
        /// and is unique across all Discord users.
        /// </summary>
        public long Id { get; set; }

       /// <summary>
       /// Represents the total amount of 
       /// experience points (XP) the user has 
       /// accumulated over their entire lifetime. 
       /// </summary>
        public long TotalXp { get; set; }

        /// <summary>
        /// Represents the amount of experience points (XP) 
        /// the user has accumulated
        /// over a specific period of time, 
        /// as specified in the bot's configuration.
        /// </summary>
        public long PeriodXp { get; set; }

        /// <summary>
        /// Represents the user's current level, 
        /// which is calculated based on their 
        /// total experience points (XP).
        /// 
        /// TODO: The way the level is calculated 
        /// and which class will be in charge of it 
        /// is yet to be decided.
        /// </summary>
        public int Level { get; set; }
    }
}
