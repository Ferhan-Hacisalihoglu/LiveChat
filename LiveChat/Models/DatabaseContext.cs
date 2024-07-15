using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Services.Description;

namespace LiveChat.Models
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Person> personTable {  get; set; }

        public DbSet<Chat> chatTable { get; set; }
    }
}