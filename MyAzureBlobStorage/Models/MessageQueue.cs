using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyAzureBlobStorage.Models
{
    public class MessageQueue
    {
        public string MessageContent { get; set; }

        public string MessageId { get; set; }

        public string MessageBody { get; set; }
    }

}