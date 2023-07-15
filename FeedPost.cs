using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParonApp
{
    internal class FeedPost
    {
        public FeedPost(string postText, string username, DateTime dateTimePosted)
        {
            PostText = postText;
            UserName = username;
            DateTimePosted = dateTimePosted;
        }
        public string PostText { get; set; }
        public DateTime DateTimePosted { get; set; }
        public string UserName { get; set; }
    }
}
