using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParonApp
{
    internal class FeedPost
    {
        public FeedPost(string postText, string user, DateTime datePosted)
        {
            PostText = postText;
            User = user;
            DatePosted = datePosted;
        }
        public string PostText { get; set; }
        public DateTime DatePosted { get; set; }
        public string User { get; set; }
    }
}
