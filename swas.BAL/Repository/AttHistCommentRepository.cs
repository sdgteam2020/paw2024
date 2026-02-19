using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using swas.BAL.DTO;
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
    public class AttHistCommentRepository : IAttHistComment
    {
        private readonly ApplicationDbContext _dbContext;

       

    }


}
