using ASPNetCoreIdentityCustomFields.Data;
using Microsoft.AspNetCore.Identity;
using swas.DAL.Models;
using swas.BAL.Utility;
using swas.DAL;
using Microsoft.EntityFrameworkCore;

namespace swas.BAL
{
    public class RepositoryUser
    {
        public readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RepositoryUser(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        public List<AddlTask> GetAdlTaskFromUsers(string actualUser)
        {
            
          
            var usersWithCustomField = _userManager.Users.Where(u => u.domain_iam == actualUser).ToList();

            List<AddlTask> unitlist = new List<AddlTask>();
            foreach (var item in usersWithCustomField)
            {
                AddlTask dt = new AddlTask();
                dt.Name = item.UserName;
                dt.Id = item.UserName;
                
                unitlist.Add(dt);
            }

            AddlTask dtS = new AddlTask();
            dtS.Name = actualUser;
            dtS.Id = actualUser;

            unitlist.Add(dtS);

            unitlist.Insert(0, new AddlTask { Id = "", Name = "--Select--" });
           
            return unitlist;
        }

        public int GetRoleId(string rolename)
        {

            Types rle = new Types();
            rle = _context.tbl_Type.FirstOrDefault(a => a.Name == rolename);
            int roleid = rle.Id;
            return roleid;


        }

        public async Task<UnitDtl> GetUnitDtl(int unitid)
        {

            UnitDtl logdet = new UnitDtl();
            logdet = await _context.tbl_mUnitBranch.FirstOrDefaultAsync(a => a.unitid== unitid);

            return logdet;

        }

    }
}
