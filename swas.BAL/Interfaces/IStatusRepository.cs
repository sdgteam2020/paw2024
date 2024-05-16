using swas.BAL.DTO;
using swas.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swas.BAL.Interfaces
{

    ///Created and Reviewed by : Sub Maj Sanal
    ///Reviewed Date : 31 Jul 23
    ///Tested By :- 
    ///Tested Date : 
    ///Start
    public interface IStatusRepository
    {
        Task<int> AddStatusAsync(tbl_mStatus status);
        Task<tbl_mStatus> GetStatusByIdAsync(int statusId);
        Task<List<tbl_mStatus>> GetAllStatusAsync();
        Task<List<DTODDLComman>> GetAllByStages_takeHolder(int ParentId,int UnitId,bool IsOwnProj);
        Task<List<DTODDLComman>> GetAll();
        Task<bool> UpdateStatusAsync(tbl_mStatus status);
        Task<bool> DeleteStatusAsync(int statusId);

    }

}
