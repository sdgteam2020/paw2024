using swas.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swas.BAL.Interfaces
{
    ///Created and Reviewed by : Sub Maj M Sanal
    ///Reviewed Date : 31 Jul 23
    ///Tested By :- 
    ///Tested Date : 
    ///Start
    public interface IStagesRepository
    {
        Task<int> AddStageAsync(tbl_mStages stage);
        Task<tbl_mStages> GetStageByIdAsync(int stageId);
        Task<List<tbl_mStages>> GetAllStagesAsync();
        Task<bool> UpdateStageAsync(tbl_mStages stage);
        Task<bool> DeleteStageAsync(int stageId);
        List<tbl_mStages> GetAllStages();
    }

}
