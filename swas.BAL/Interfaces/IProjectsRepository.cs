using swas.BAL.DTO;
using swas.DAL;
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
    public interface IProjectsRepository
    { 
        Task<int> AddProjectAsync(tbl_Projects project);
        Task<tbl_Projects> GetProjectByIdAsync(int projectId);
        Task<bool> UpdateProjectAsync(tbl_Projects project);
        Task<List<tbl_Projects>> GetAllProjectsAsync();
        Task<bool> DeleteProjectAsync(int projectId);
        Task<List<DTOProjectsFwd>> GetActInboxAsync();
        Task<List<DTOProjectsFwd>> GetDashboardStatusDetails(int StatuId,int UnitId);
        Task<List<DTOProjectsFwd>> GetActSendItemsAsync();
        Task<List<tbl_Projects>> GetActComplettemsAsync();
        Task<List<tbl_Projects>> GetActDraftItemsAsync();
        Task<List<tbl_Projects>> GetStatusProjAsync(int statusid);
        Task<tbl_Projects> GetProjectByPsmIdAsync(int psmId);
        Task<List<tbl_Projects>> GetMyProjectsAsync();
        Task<List<tbl_Projects>> GetActProjectsAsync();
        Task<tbl_ProjStakeHolderMov> GettXNByPsmIdAsync(int psmId);
        Task<List<DToWhiteListed>> GetWhiteListedActionProj();
        Task<List<tbl_Projects>> GetProcProjAsync();
        Task<List<tbl_Projects>> GetProjforCommentsAsync();
        Task<List<ProjHistory>> GetProjectHistorybyID(int? dtaProjID);
        Task<bool> UpdateTxnAsync(tbl_ProjStakeHolderMov psmov);
        Task<tbl_Projects> GetProjectByIdAsync1(int? dataProjId);
    }



}
