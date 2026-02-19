using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using swas.BAL.DTO;
using swas.DAL.Models;

namespace swas.BAL.Interfaces
{
    public interface IRemainder
    {
        Task<int> AddRemainder(int psmid, int Psmid, int fromUnitId, int toUnitId, string remarks, string userDetails);
        Task<List<RemainderDisplayDto>> ProjectRemainderMovHistory(int? ProjectId);

        Task<int> GetProjectById(string? ProjName);
        Task<int> UpdateReaminderRead(int? Projid,int pullback);
        Task<List<RemainderDisplayDto>> GetAllAsync();
       
    }
}   
