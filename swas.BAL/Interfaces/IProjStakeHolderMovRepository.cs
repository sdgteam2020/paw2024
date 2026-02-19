using swas.BAL.DTO;
using swas.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swas.BAL.Interfaces
{
    public interface IProjStakeHolderMovRepository:IGenericRepositoryDL<tbl_ProjStakeHolderMov> 
    {
        Task<int> AddProjStakeHolderMovAsync(tbl_ProjStakeHolderMov psmove);
        Task<DTOProjectMovHistory> ProjectMovHistory(int? ProjectId);

        Task<int> GetProjectId(string? ProjName);
        Task<List<DTOProjectsFwd>> ProjectMovement(int? ProjectId);
        Task<List<DTOProjectHold>> ProjectHolsTimeCalculate(int ProjectId );
        int GetLastRecProjectMov(int? ProjectId);
        Task<int> GetLastRecProjectMovForUnod(int ProjectId,int? TounitId);
        Task<DTODashboard> DashboardCount(int UserId);
        Task<DTOChartSummarylist> CreateChartSummary(int UserId);
       

        Task<bool> CheckFwdCondition(int ProjId, int StatusId,string Actionsname); 

        
       

        Task<tbl_ProjStakeHolderMov> GetProjStakeHolderMovByIdAsync(int psmId);
        Task<List<tbl_ProjStakeHolderMov>> GetAllProjStakeHolderMovAsync();
        Task<bool> UpdateProjStakeHolderMovAsync(tbl_ProjStakeHolderMov projStakeHolderMov);
        Task<bool> DeleteProjStakeHolderMovAsync(int psmId);
        Task<int> CountinboxAsync(int stkhol);
       

        Task<int> AddProStkMovBlogAsync(Projmove psmove);
        Task<int> ValStatusAsync(int? ProjId);
        Task<int> GetlaststageId(int? ProjId);
        Task<int> ReturnDuplProjMovAsync(Projmove psmove);
        Task<int> RetWithObsnMovAsync(Projmove psmove);
        Task<List<ProjLogView>> GetProjLogviewAsync(string startDate, string endDate);

        string GetSponsorUnitName(int StakeHolderId);


        Task<int> AddNotificationCommentAsync(Notification notifications);
        Task<string> CheckPreviousApprovals(int statusId, int projId, int actionsId);


	}


}
