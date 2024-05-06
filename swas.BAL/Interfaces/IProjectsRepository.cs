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
        Task<tbl_Projects> GetProjectByIdAsync1(int dataProjId);
      

        Task<List<tbl_Projects>> GetProjByIdLstAsync(int projectId);

        Task<List<tbl_Projects>> GetAllProjectsAsync();
        Task<List<tbl_Projects>> GetActProjectsAsync();
        Task<List<tbl_Projects>> GetActInboxAsync(); 
        Task<List<tbl_Projects>> GetActSendItemsAsync();
        Task<List<tbl_Projects>> GetActComplettemsAsync();
        Task<bool> UpdateProjectAsync(tbl_Projects project);

        Task<bool> UpdateTxnAsync(tbl_ProjStakeHolderMov psmov);
        Task<bool> DeleteProjectAsync(int projectId);
        Task<List<ProjHistory>> GetProjectHistory(string userid);
        Task<List<ProjHistory>> GetProjectHistorybyID(int? dtaProjID);

        Task<List<ProjHistory>> GetProjectHistorybyID1(int? dataProjId);

        List<tbl_AttHistory> GetAttachmentsByPsmId(int psmId);
        bool VerifyProjectNameAsync(string projname);
        Task<List<tbl_Projects>> GActProjectsAsync();
        Task<tbl_Projects> GetProjbyprojCode(string ProjCode);
        Task<tbl_Projects> GetProjectByPsmIdAsync(int psmId);

        Task<tbl_ProjStakeHolderMov> GettXNByPsmIdAsync(int psmId);
        Task<List<TimeExceeds>> DelayedProj();
        Task<List<DToWhiteListed>> GetWhiteListedActionProj();
        Task<List<TimeExceeds>> GetRecentActionProj();
        Task<List<TimeExceeds>> GetHoldActionProj();

        Task<List<TimeExceeds>> GetHoldRFPProj();
        Task<List<TimeExceedsAlerts>> GetStkHolderStatus();
        Task<bool> UndoChanges(int id);
        Task<List<tbl_Projects>> GetWhitelistedProjAsync();

        Task<List<tbl_Projects>> GetActDraftItemsAsync();

        Task<string> ValidateActionSel(int? ProjID, int? ActionId, int? StatusId);

        int CurrentPsmGet(int? ProjID);


        Task<List<tbl_Projects>> GetStatusProjAsync(int statusid);
        Task<List<tbl_Projects>> GetProcProjAsync();


        Task<List<CommentBy_StakeHolder>> GetCommentByStakeHolder(int? projid);

        Task<List<tbl_Projects>> GetMyProjectsAsync();


        Task<List<tbl_Projects>> GetProjforCommentsAsync();

        Task<List<NextActionsDto>> NextActionGet(int projid);

        Task<List<tbl_Projects>> GetProjforEditAsync();
        Task<tbl_Projects> EditWithHistory(int proid);
        Task<bool> EdtSaveProjAsync(tbl_Projects project);

        Task<List<tbl_Projects>> GetProjforDocView();

        Task<bool> UserRegistered(string UserName);

    }



}
