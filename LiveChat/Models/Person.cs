using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Services.Description;

namespace LiveChat.Models
{
    [Table("Person")]
    public class Person
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(30), Required]
        public string userName { get; set; }

        public virtual IList<Chat> chatTable { get; set; }

        [Required]
        public DateTime lastChatDate { get; set; }

    }
}