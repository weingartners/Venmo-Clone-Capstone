﻿using System.Collections.Generic;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface ITransferDao
    {
        public bool SaveTransfer(int sendingUserId, decimal transferAmount, int recievingUserId, int typeId, int statusId);
        public List<Transfer> GetTransfersById(int id);

    }
}
