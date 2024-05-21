﻿using swas.BAL.DTO;
using swas.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swas.BAL.Interfaces
{
    public interface IActionsRepository : IGenericRepositoryDL<tbl_mActions>
    {


        Task<List<DTODDLComman>> GetActionByStatusId(int StatusId);
        Task<List<DTODDLComman>> GetActionsMappingIdByStatusId(int StatusId);


    }
}
