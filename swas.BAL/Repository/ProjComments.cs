using Microsoft.AspNetCore.DataProtection;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using swas.BAL.DTO;
using swas.BAL.Interfaces;
using swas.DAL;
using swas.DAL.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace swas.BAL
{
    public class ProjComments : IProjComments
    {
        private readonly ApplicationDbContext _context;
        private readonly IDataProtector _dataProtector;
        public ProjComments(ApplicationDbContext applicationDbContext, IDataProtectionProvider dataProtector)
        {
            _context = applicationDbContext;
            _dataProtector = dataProtector.CreateProtector("swas.UI.Controllers.ProjectsController"); 
        }
        public async Task<List<DTOProComments>> GetAllStkForComment(int UnitId, int StatusId)
        {
           

            #region GetAllStkForCommentWithProc
            try
            {
                List<DTOProComments> results = new List<DTOProComments>();

                using (SqlConnection conn = new SqlConnection(_context.Database.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("usp_GetAllStkForComment", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UnitId", UnitId);
                        cmd.Parameters.AddWithValue("@StatusId", StatusId);

                        await conn.OpenAsync();
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                DTOProComments item = new DTOProComments
                                {
                                    ProjId = Convert.ToInt32(reader["ProjId"]),
                                    PsmId = Convert.ToInt32(reader["PsmId"]),
                                    ProjectName = reader["ProjectName"].ToString(),
                                    Stakeholder = reader["Stakeholder"].ToString(),
                                    Status = reader["Status"].ToString(),
                                    StkStatusId = Convert.ToInt32(reader["StkStatusId"]),
                                    UnitId = Convert.ToInt32(reader["UnitId"]),
                                    TimeStamp = Convert.ToDateTime(reader["TimeStamp"]),
                                    IsComment = Convert.ToBoolean(reader["IsComment"]),
                                    AdminApprovalStatus = Convert.ToBoolean(reader["AdminApprovalStatus"])
                                };

                                item.EncyID = _dataProtector.Protect(item.ProjId.ToString());
                                results.Add(item);
                            }
                        }
                    }
                }
                return results.OrderByDescending(i => i.TimeStamp).ToList();

            }catch(Exception ex)
            {
                throw;
            }

            #endregion
        }


        public async Task<DTOProComments> GetCommentStatus(int UnitId)
        {
            var tempFileName = await (from comment in _context.StkComment
                                      join status in _context.StkStatus on comment.StkStatusId equals status.StkStatusId into statusGroup
                                      from status in statusGroup.DefaultIfEmpty()
                                      where comment.StakeHolderId == UnitId
                                      select new DTOProComments
                                      {
                                          Status = status.Status,
                                          StkStatusId = comment.StkStatusId,
                                      }).OrderByDescending(c => c.StkCommentId).SingleOrDefaultAsync();
         
            return null;
        }

        public async Task<List<DTOProComments>> FindForComment(int? UnitId, string searchQuery)
        {


            #region GetAllStkForCommentWithProc

            List<DTOProComments> results = new List<DTOProComments>();

            using (SqlConnection conn = new SqlConnection(_context.Database.GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("usp_GetAllComment", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UnitId", UnitId);
                    cmd.Parameters.AddWithValue("@ProjectName", searchQuery);


                    await conn.OpenAsync();
                    DataTable dt = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    var data = dt;
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {


                        while (await reader.ReadAsync())
                        {
                            DTOProComments item = new DTOProComments
                            {
                                ProjId = Convert.ToInt32(reader["ProjId"]),
                                PsmId = Convert.ToInt32(reader["PsmId"]),
                                ProjectName = reader["ProjectName"].ToString(),
                                Stakeholder = reader["Stakeholder"].ToString(),
                                Status = reader["Status"].ToString(),
                                StkStatusId = Convert.ToInt32(reader["StkStatusId"]),
                                UnitId = Convert.ToInt32(reader["UnitId"]),
                                TimeStamp = Convert.ToDateTime(reader["TimeStamp"]),
                                IsComment = Convert.ToBoolean(reader["IsComment"]),
                                AdminApprovalStatus = Convert.ToBoolean(reader["AdminApprovalStatus"])
                            };

                            item.EncyID = _dataProtector.Protect(item.ProjId.ToString());
                            results.Add(item);
                        }
                    }
                }
            }
            return results.ToList();

            #endregion
        }

    }
}
