using swas.BAL.DTO;
using swas.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swas.BAL.Interfaces
{
    public interface IActionsRepository
    {

        ///Created and Reviewed by : Sub Maj Sanal
        ///Reviewed Date : 31 Jul 23
        ///Tested By :- 
        ///Tested Date : 
        ///Start

        Task<int> AddActionsAsync(tbl_mActions actions);
        Task<tbl_mActions> GetActionsByIdAsync(int actionsId);
        Task<List<Actiondto>> GetAllActionsAsync();
        Task<bool> UpdateActionsAsync(tbl_mActions actions);
        Task<bool> DeleteActionsAsync(int actionsId);

        Task<string> ValidateActionsAsync(int actionsId, int? stkholId);

        Task<List<ProjectDetailsDTO>> GetNextStgStatAct(int? ProjID, int? psmID, int? stgID);

        Task<List<ActionsSeq>> GetActionresp();
    }
}
