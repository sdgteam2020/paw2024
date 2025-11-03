using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using swas.BAL.Interfaces;
using swas.DAL.Models;
using swas.DAL;
using static swas.DAL.Models.LegacyHistory;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;

namespace swas.BAL.Repository
{
    public class DateApprovalRepository : IDateApprovalRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IDataProtector _dataProtector;
        public DateApprovalRepository(ApplicationDbContext context, IDataProtectionProvider dataProtector)
        {
            _context = context;
            _dataProtector = dataProtector.CreateProtector("swas.UI.Controllers.ProjectsController");
        }

        public List<DateApproval> GetDateApprovalList()
        {
            try
            {
                #region OldLogic
                //var data = (from da in _context.DateApproval
                //            join p in _context.Projects on da.ProjId equals p.ProjId
                //            join u in _context.tbl_mUnitBranch on da.UnitId equals u.unitid
                //            join lh in _context.LegacyHistory
                //       .Where(lh => lh.ActionType == ActionTypeEnum.RequestSent)
                //       on da.ProjId equals lh.ProjectId into lhJoin
                //            from LH in lhJoin.DefaultIfEmpty() 

                //            orderby da.Request_Date descending
                //            group new { da, p, u, LH } by new { da.ProjId, p.ProjName } into grouped
                //            select new DateApproval
                //            {
                //                Id = da.Id,
                //                ProjId = da.ProjId,
                //                UnitId = da.UnitId,
                //                Request_Date = da.Request_Date,
                //                DDGIT_Approval_dat = da.DDGIT_Approval_dat,
                //                DDGIT_approval = da.DDGIT_approval,
                //                ProjName = p.ProjName,
                //                UnitName = u.UnitName,
                //                User = da.User,
                //                IsRead = da.IsRead,
                //                Remarks = LH != null ? LH.Remarks : null


                //            }).ToList();
                #endregion
                var data = (from da in _context.DateApproval
                            where da.RequestType == 1
                            join p in _context.Projects on da.ProjId equals p.ProjId
                            join u in _context.tbl_mUnitBranch on da.UnitId equals u.unitid
                            join lh in _context.LegacyHistory
                            .Where(lh => lh.ActionType == ActionTypeEnum.RequestSent)
                            on da.ProjId equals lh.ProjectId into lhJoin
                            from LH in lhJoin.DefaultIfEmpty()

                            group new { da, p, u, LH } by da.ProjId into grouped
                            select new DateApproval
                            {
                                Id = grouped.OrderByDescending(x => x.da.Request_Date).FirstOrDefault().da.Id,
                                ProjId = grouped.Key,
                                UnitId = grouped.OrderByDescending(x => x.da.Request_Date).FirstOrDefault().da.UnitId,
                                Request_Date = grouped.OrderByDescending(x => x.da.Request_Date).FirstOrDefault().da.Request_Date,
                                DDGIT_Approval_dat = grouped.OrderByDescending(x => x.da.Request_Date).FirstOrDefault().da.DDGIT_Approval_dat,
                                DDGIT_approval = grouped.OrderByDescending(x => x.da.Request_Date).FirstOrDefault().da.DDGIT_approval,
                                ProjName = grouped.OrderByDescending(x => x.da.Request_Date).FirstOrDefault().p.ProjName,
                                UnitName = grouped.OrderByDescending(x => x.da.Request_Date).FirstOrDefault().u.UnitName,
                                User = grouped.OrderByDescending(x => x.da.Request_Date).FirstOrDefault().da.User,
                                IsRead = grouped.OrderByDescending(x => x.da.Request_Date).FirstOrDefault().da.IsRead,
                                Remarks = grouped.OrderByDescending(x => x.LH.ActionDate).FirstOrDefault().LH.Remarks,
                                EncyID = _dataProtector.Protect(grouped.Key.ToString()),

                            }).OrderByDescending(x => x.Request_Date).ToList();


                return data;
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        public List<DateApproval> GetDateApprovalListForAdmin()
        {
            try
            {
                #region OldLogic

                #endregion
                var data = (from da in _context.DateApproval
                            where da.RequestType == 2
                            join p in _context.Projects on da.ProjId equals p.ProjId
                            join u in _context.tbl_mUnitBranch on da.UnitId equals u.unitid
                            join lh in _context.LegacyHistory
                            .Where(lh => lh.ActionType == ActionTypeEnum.RequestSent)
                            on da.ProjId equals lh.ProjectId into lhJoin
                            from LH in lhJoin.DefaultIfEmpty()
                            select new { da, p, u, LH })
              .AsEnumerable() // Move to client-side evaluation
              .GroupBy(x => x.da.ProjId)
              .Select(grouped => new DateApproval
              {
                  Id = grouped.OrderByDescending(x => x.da.Request_Date).FirstOrDefault().da.Id,
                  ProjId = grouped.Key,
                  UnitId = grouped.OrderByDescending(x => x.da.Request_Date).FirstOrDefault().da.UnitId,
                  Request_Date = grouped.OrderByDescending(x => x.da.Request_Date).FirstOrDefault().da.Request_Date,
                  DDGIT_Approval_dat = grouped.OrderByDescending(x => x.da.Request_Date).FirstOrDefault().da.DDGIT_Approval_dat,
                  DDGIT_approval = grouped.OrderByDescending(x => x.da.Request_Date).FirstOrDefault().da.DDGIT_approval,
                  ProjName = grouped.OrderByDescending(x => x.da.Request_Date).FirstOrDefault().p.ProjName,
                  UnitName = grouped.OrderByDescending(x => x.da.Request_Date).FirstOrDefault().u.UnitName,
                  User = grouped.OrderByDescending(x => x.da.Request_Date).FirstOrDefault().da.User,
                  IsRead = grouped.OrderByDescending(x => x.da.Request_Date).FirstOrDefault().da.IsRead,
                  EncyID = _dataProtector.Protect(grouped.Key.ToString()),

                  Remarks = grouped.OrderByDescending(x => x.da.Request_Date).FirstOrDefault().LH != null ? grouped.OrderByDescending(x => x.da.Request_Date).FirstOrDefault().LH.Remarks : null
              })
              .OrderByDescending(x => x.Request_Date).ToList();




                return data;
            }
            catch (Exception ex)
            {

                return null;
            }
        }
    }


   
}


