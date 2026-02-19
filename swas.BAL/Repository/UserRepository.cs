using Microsoft.IdentityModel.Tokens;
using swas.DAL.Models;
using swas.BAL.Utility;
using swas.BAL.Interfaces;
using swas.DAL;
using Microsoft.EntityFrameworkCore;
using ASPNetCoreIdentityCustomFields.Data;

namespace swas.BAL
{
    public class UserRepository : IUserRepository
    {
        public readonly ApplicationDbContext _dbContext;
       

        public UserRepository(ApplicationDbContext context)
        {
            _dbContext = context;

        }

        public bool CheckUserExist(string UserName)
        {
            return _dbContext.Users.Any(e => e.UserName == UserName);
            
        }



      
        public async Task<List<ApplicationUser>> GetAllUsersAsync()
        {
            var users = await _dbContext.Users.ToListAsync();
            return users;
        }

     

        public async Task<int> Add(tbl_LoginLog userlog)
        {
           
                _dbContext.tbl_LoginLog.Add(userlog);
                await _dbContext.SaveChangesAsync();
                return 1;
           
        }
    }
}
