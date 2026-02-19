using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

using swas.BAL.Interfaces;
using swas.DAL;
using swas.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swas.BAL.Repository
{
    public class ChartService : IChartService
    {
        private readonly ApplicationDbContext _dbContext;

        public ChartService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<List<ChartModelS>> GetChartDataS()
        {
            try
            {
                var chartModels = await _dbContext.Set<ChartModelS>()
                    .FromSqlRaw("EXEC getMonthInOutSummary")
                    .ToListAsync();

                return chartModels;
            }
            catch (Exception ex)
            {
                string ss = ex.Message;
                return new List<ChartModelS>();
            }
        }

        public async Task<List<ChartModel>> GetChartData()
        {
            try
            {
                var chartModels = await _dbContext.Set<ChartModel>()
                    .FromSqlRaw("EXEC getAppTypeCounts")
                    .ToListAsync();

                return chartModels;
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
                return new List<ChartModel>();
            }
        }


    }

}
