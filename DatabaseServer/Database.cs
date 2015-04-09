using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;

namespace DatabaseServer
{
    [Table(Name = "Accounts")]
    public class Account
    {
        [Column(IsPrimaryKey = true)]
        public long ID;
        [Column]
        public string Name;
        [Column]
        public string Password;
        [Column]
        public string Email;
        [Column]
        public Int16 Permission;
        [Column]
        public DateTime LastLogin;
        [Column]
        public DateTime RegistrationDate;
        [Column]
        public bool Active;
        [Column]
        public string Ticket;
    }

    [Table(Name = "Alias")]
    public class Alias
    {
        [Column(IsPrimaryKey = true)]
        public long ID;
        [Column]
        public long AccountID;
        [Column]
        public string Name;
        [Column]
        public DateTime CreationDate;
        [Column]
        public string IPAddress;
        [Column]
        public long TimePlayed;
    }

    [Table(Name = "Player")]
    public class Players
    {
        [Column(IsPrimaryKey = true)]
        public long ID;
        [Column]
        public long AliasID;
        [Column]
        public long ZoneID;
        [Column]
        public long StatsID;
        [Column]
        public string Inventory;
        [Column]
        public string Skills;
        [Column]
        public byte[] Banner;
        [Column]
        public int SkillID;
    }

    [Table(Name = "Stats")]
    public class Stats
    {
        [Column(IsPrimaryKey = true)]
        public long ID;
        [Column]
        public long ZoneID;
        [Column]
        public int Cash;
        [Column]
        public int Experience;
        [Column]
        public int EperienceTotal;
        [Column]
        public int Kills;
        [Column]
        public int Deaths;
    }

    [Table(Name = "Zone")]
    public class Zones
    {
        [Column(IsPrimaryKey = true)]
        public long ID;
        [Column]
        public string Password;
        [Column]
        public string Name;
        [Column]
        public string Description;
        [Column]
        public bool Active;
        [Column]
        public string IP;
        [Column]
        public string Port;
        [Column]
        public bool Advanced;
    }

    public class Database : DataContext
    {
        public Table<Account> Accounts;
        public Table<Alias> Aliases;
        public Table<Players> Players;
        public Table<Stats> Stats;
        public Table<Zones> Zones;

        private const string _connectionString = "Server=Brian-Pc\\INFANTRY;Database=Data;Trusted_Connection=True";                   //SQL connection string

        public bool _allowMultipleClients;      //More than one client from same IP

        public Database()
            : base(_connectionString)
        {
        }
    }
}
