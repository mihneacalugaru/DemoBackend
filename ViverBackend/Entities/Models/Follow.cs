using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ViverBackend.Entities.Models
{
    public class Follow
    {
        public int Id { get; set; }

        [ForeignKey("Follows")]
        public int FollowsId { get; set; }

        [ForeignKey("Followed")]
        public int FollowedById { get; set; }

        public DateTime DateOfFollow { get; set; }

        public User Follows { get; set; }

        public User FollowedBy { get; set; }
    }
}
