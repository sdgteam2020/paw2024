using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using swas.BAL.Interfaces;
using swas.DAL.Models;
using swas.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace swas.BAL.Repository
{
    public class ProjStakeHolderCcMovRepository : GenericRepositoryDL<tbl_ProjStakeHolderCcMov>, IProjStakeHolderCcMovRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDataProtector _dataProtector;

        public ProjStakeHolderCcMovRepository(ApplicationDbContext dbContext, IDataProtectionProvider DataProtector, IHttpContextAccessor httpContextAccessor) : base(dbContext)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _dataProtector = DataProtector.CreateProtector("swas.UI.Controllers.ProjectsController");

        }

        public async Task<tbl_ProjStakeHolderCcMov> GetdataBuPsmiandTounitId(int psmId, int TounitId)
        {
            return await _dbContext.ProjStakeHolderCcMov.FirstOrDefaultAsync(a => a.PsmId == psmId && a.ToCcUnitId == TounitId);
        }
    }
    }
