using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using swas.DAL.Models;

namespace swas.BAL.Interfaces
{
    public interface ILegacyHistoryRepository
    {
        Task AddHistoryAsync(LegacyHistory history);
        Task<IEnumerable<LegacyHistory>> GetHistoryByProjectIdAsync(int projectId);
    }
}
