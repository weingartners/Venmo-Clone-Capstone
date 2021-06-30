using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoServer.Models
{
    public class Transfer
    {
        public int SendingUserId { get; set; }
        public decimal TransferAmount { get; set; }
        public int RecievingUserId { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
    }
}
