using Grpc.Core;
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
  
    public class ActionsRepository : GenericRepositoryDL<tbl_mActions>, IActionsRepository

    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ActionsRepository(ApplicationDbContext context) : base(context)
        {
            _dbContext = context;
            
        }

        public async Task<tbl_mActions> getActionByName(string name)
        {
            return _dbContext.mActions.FirstOrDefault(a => a.Actions == name);
        }

        ///Created and Reviewed by : Sub Maj Sanal
        ///Reviewed Date : 31 Jul 23

        ///Created and Reviewed by : Sub Maj Sanal
        ///Reviewed Date : 31 Jul 23

        ///Created by : Ajay
        ///Reviewed Date : 30 Aug 23
        ///Revied by Sub Maj Sanal
        ///01 Sep 23









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

        public Task<List<ActionsSeq>> GetActionresp()
        {
            throw new NotImplementedException();
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




        //public async Task<List<ActionsSeq>> GetActionresp()
        //{


        //    var query = from a in _dbContext.mActions
        //                join sam in _dbContext.TrnStatusActionsMapping on a.ActionsId equals sam.ActionsId
        //                join usm in _dbContext.TrnUnitStatusMapping on sam.StatusId equals usm.StatusId
        //                join c in _dbContext.tbl_mUnitBranch on usm.UnitId equals c.unitid into unitGroup
        //                from c in unitGroup.DefaultIfEmpty()
        //                join e in _dbContext.mStages on a. equals e.StagesId into stagesGroup
        //                from e in stagesGroup.DefaultIfEmpty()
        //                join d in _dbContext.mStatus on a.StatusId equals d.StatusId into statusGroup
        //                from d in statusGroup.DefaultIfEmpty()
        //                where c.UnitName != null
        //                orderby a.Actionseq
        //                select new ActionsSeq
        //                {
        //                    Stages = e.Stages,
        //                    Status = d.Status,
        //                    Actions = a.Actions,
        //                    ActionDesc = a.ActionDesc,
        //                    UnitName = c.UnitName
        //                };


        //    var actionsList = await query.ToListAsync();



        //    return actionsList;


        //}

    }


}
