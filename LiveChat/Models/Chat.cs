using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LiveChat.Models
{
    [Table("Chat")]
    public class Chat
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [StringLength(200),Required]
        public string message { get; set; }

        [Required]
        public int toUserId { get; set; }

        [Required]
        public DateTime date { get; set; }

    }
}