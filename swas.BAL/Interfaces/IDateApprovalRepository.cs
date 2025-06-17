using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using swas.DAL.Models;

namespace swas.BAL.Interfaces
{
    public interface IDateApprovalRepository
    {
        List<DateApproval> GetDateApprovalList();
    }
}
