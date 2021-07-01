using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoServer.Models
{
    public class Transfer
    {
        public int TransferId { get; set; }
        public int SendingUserId { get; set; }
        public decimal TransferAmount { get; set; }
        public int ReceivingUserId { get; set; }
        
        public int TypeId { get; set; }
        public int StatusId { get; set; }
        public string FromUserName { get; set; }
        public string ToUserName { get; set; }
    }
}
