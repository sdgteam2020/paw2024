using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using swas.BAL.DTO;
using swas.BAL.Helpers;
using swas.BAL.Interfaces;
using swas.DAL;
using swas.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace swas.BAL.Repository
{

    ///Created and Reviewed by : Sub Maj Sanal
    ///Reviewed Date : 31 Jul 23
    ///Tested By :- 
    ///Tested Date : 
    ///Start
    public class ActionsRepository : IActionsRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ActionsRepository(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        ///Created and Reviewed by : Sub Maj Sanal
        ///Reviewed Date : 31 Jul 23
        public async Task<int> AddActionsAsync(tbl_mActions actions)
        {
            var maxActionSequence = await _dbContext.mActions.MaxAsync(a => (int?)a.Actionseq) ?? 0;

            // Increment the maximum ActionSequence value by 1 for the new actions object
            actions.Actionseq = maxActionSequence + 1;

            _dbContext.mActions.Add(actions);
            await _dbContext.SaveChangesAsync();
            return actions.ActionsId;
        }
        ///Created and Reviewed by : Sub Maj Sanal
        ///Reviewed Date : 31 Jul 23
        public async Task<tbl_mActions> GetActionsByIdAsync(int actionsId)
        {
            return await _dbContext.mActions.FindAsync(actionsId);
        }
        ///Created and Reviewed by : Sub Maj Sanal
        ///Reviewed Date : 31 Jul 23
        public async Task<List<Actiondto>> GetAllActionsAsync()
        {
           
            
            List<Actiondto> actionsList = new List<Actiondto>();

            var query = from a in _dbContext.mActions
                        join b in _dbContext.mStatus on a.StatusId equals b.StatusId into statusGroup
                        from b in statusGroup.DefaultIfEmpty()
                        join c in _dbContext.mStages on a.StagesId equals c.StagesId into stagesGroup
                        from c in stagesGroup.DefaultIfEmpty()
                        where a.StagesId<70 && a.StatusId<70
                        orderby (b != null ? b.StatusId : 0), (c != null ? c.StagesId : 0)
                        select new Actiondto
                        {
                            ActionsId = a.ActionsId,
                            Status = b != null ? b.Status : null,
                            Stages = c != null ? c.Stages : null,
                            Actions = a.Actions,
                            TimeLimit = a.TimeLimit,
                            UnitName = "",
                            ActionSeq = a.Actionseq,
                            EditDeleteDate = a.EditDeleteDate,
                            Statuss = a.IsActive == true ? "Active" : "Deleted"
                        };

            actionsList = query.ToList();



            return actionsList;
            
        }
        ///Created by : Ajay
        ///Reviewed Date : 30 Aug 23
        ///Revied by Sub Maj Sanal
        ///01 Sep 23
        public async Task<bool> UpdateActionsAsync(tbl_mActions actions)
        {

            //_dbContext.Entry(actions).State = EntityState.Modified;
            actions.IsDeleted = false;
            actions.IsActive = true;
            actions.UpdatedByUserId = "6";
            actions.DateTimeOfUpdate = DateTime.Now;
            actions.EditDeleteDate = DateTime.Now;
            actions.EditDeleteBy = "asdc";
            //actions.TimeLimit = 30;
            _dbContext.mActions.Update(actions);
            await _dbContext.SaveChangesAsync();
            return true;
        }




        public async Task<bool> DeleteActionsAsync(int id)
        {
            try
            {
                var existingActions = await _dbContext.mActions.FindAsync(id);

                if (existingActions == null)
                {
                    return false; // Entity not found
                }

                _dbContext.mActions.Remove(existingActions);
                await _dbContext.SaveChangesAsync();

                return true; // Successfully deleted
            }
            catch (Exception)
            {
                return false; // Delete failed
            }
        }

        public async Task<string> ValidateActionsAsync(int actionsId, int? stkholId)
        {
            var query = from a in _dbContext.mActions
                        join b in _dbContext.tbl_mUnitBranch on a.StatCompId equals b.unitid into unitJoin
                        from b in unitJoin.DefaultIfEmpty()
                        where a.ActionsId == actionsId && a.StatCompId > 0 && a.StatCompId != stkholId
                        select "This Action is authorised for " + b.UnitName;

            string ss = await query.FirstOrDefaultAsync();

            return ss;
        }

    //    select pm.StageId, pm.StatusId, pm.actionid, (select ActionsId from mActions where StatCompId= 4 and StagesId = 2



    //and ActionsId not in (select actionid from ProjStakeHolderMov where projid= 178)
    //        ) as nextactionid,
    //              (select StatusId from mActions where StatCompId=4 and StagesId = 2 ) as nextstatus,
    //              (select StagesId from mActions where StatCompId=4 and StagesId = 2  ) as nextstage,

    //               pj.*
    //               from Projects pj
    //              left join ProjStakeHolderMov pm
    //              on pj.CurrentPslmId = pm.PsmId
    //              left join mActions ma on
    //              pm.ActionId = ma.ActionsId
    //              where pm.StageId = 2 and pj.ProjId= 178





        //select pm.StageId, pm.StatusId, pm.actionid, (select ActionsId from mActions where StatCompId= 4 and StageId = 2) as nextactionid,
        //(select StatusId from mActions where StatCompId=4 and StageId = 2 ) as nextstatus,
        //(select StageId from mActions where StatCompId=4 and StageId = 2  ) as nextstage,

        // pj.*
        // from Projects pj
        //left join ProjStakeHolderMov pm
        //on pj.CurrentPslmId = pm.PsmId
        //left join mActions ma on
        //pm.ActionId = ma.ActionsId
        //where pm.StageId = 2 and pj.ProjId= 111



        public async Task<List<ProjectDetailsDTO>> GetNextStgStatAct(int? ProjID, int? psmID, int? stgID)
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");


            var projectDetailsList = (from pj in _dbContext.Projects
                                      join pm in _dbContext.ProjStakeHolderMov on pj.ProjId equals pm.ProjId into pmJoin
                                      from pm in pmJoin.DefaultIfEmpty()
                                      join actm in _dbContext.TrnStatusActionsMapping on pm.StatusActionsMappingId equals actm.StatusActionsMappingId 
                                      join ma in _dbContext.mActions on actm.ActionsId equals ma.ActionsId into maJoin
                                      from ma in maJoin.DefaultIfEmpty()
                                      where pj.ProjId == ProjID 


            select new ProjectDetailsDTO
                                      {
                                          //StageId = pm.StageId,
                                          StatusId = actm.StatusId,
                                          ActionId = actm.ActionsId,
                                          NextActionId = _dbContext.mActions
                                              .Where(a => a.StatCompId == Logins.unitid && a.StagesId == 2 && !_dbContext.ProjStakeHolderMov.Any(psh => psh.ProjId == ProjID))
                                              .Select(a => (int?)a.ActionsId)
                                              .FirstOrDefault()??0,
                                          NextStatus = _dbContext.mActions
                                              .Where(a => a.StatCompId == Logins.unitid && a.StagesId == 2)
                                              .Select(a => (int?)a.StatusId)
                                              .FirstOrDefault(),
                                          NextStage = _dbContext.mActions
                                              .Where(a => a.StatCompId == Logins.unitid && a.StagesId == 2)
                                              .Select(a => (int?)a.StagesId)
                                              .FirstOrDefault(),
                                          StkHoldID = pj.StakeHolderId,
                                          ActionName = _dbContext.mActions
                                .Where(a => a.StatCompId == Logins.unitid && a.StagesId == 2 && !_dbContext.ProjStakeHolderMov.Any(psh => psh.ProjId == ProjID ))
                                .Select(a => a.Actions)
                                .FirstOrDefault()
                                      }).ToList();






            return projectDetailsList;
        }





        public async Task<List<ActionsSeq>> GetActionresp()
        {


            var query = from a in _dbContext.mActions
                        join c in _dbContext.tbl_mUnitBranch on a.StatCompId equals c.unitid into unitGroup
                        from c in unitGroup.DefaultIfEmpty()
                        join e in _dbContext.mStages on a.StagesId equals e.StagesId into stagesGroup
                        from e in stagesGroup.DefaultIfEmpty()
                        join d in _dbContext.mStatus on a.StatusId equals d.StatusId into statusGroup
                        from d in statusGroup.DefaultIfEmpty()
                        where c.UnitName != null
                        orderby a.Actionseq
                        select new ActionsSeq
                        {
                            Stages = e.Stages,
                            Status = d.Status,
                            Actions = a.Actions,
                            ActionDesc = a.ActionDesc,
                            UnitName = c.UnitName
                        };


            var actionsList = await query.ToListAsync();



            return actionsList;


        }

        public async Task<List<DTODDLComman>> GetActionByStatusId(int StatusId)
        {
            var query = await (from act in _dbContext.mActions
                        join map in _dbContext.TrnStatusActionsMapping on act.ActionsId equals map.ActionsId
                        where map.StatusId == StatusId 
                        select new DTODDLComman
                        {
                            Id=act.ActionsId,
                            Name=act.Actions,

                        }).ToArrayAsync();
            return query.ToList();
        }
        public async Task<List<DTODDLComman>> GetActionsMappingIdByStatusId(int StatusId)
        {
            var query = await (from act in _dbContext.mActions
                               join map in _dbContext.TrnStatusActionsMapping on act.ActionsId equals map.ActionsId
                               where map.StatusId == StatusId
                               select new DTODDLComman
                               {
                                   Id = map.StatusActionsMappingId,
                                   Name = act.Actions,

                               }).ToArrayAsync();
            return query.ToList();
        }
    }


}
