using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using swas.DAL;
using swas.BAL.Interfaces;

namespace swas.BAL
{
    public class SoftwareTypeRepository : ISoftwareTypeRepository
    {
        private readonly ApplicationDbContext _context;

        public SoftwareTypeRepository(ApplicationDbContext context)
        {
            _context = context;
        }
     
        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }

      
    }
}
