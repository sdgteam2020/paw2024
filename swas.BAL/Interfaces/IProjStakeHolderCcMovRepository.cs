using swas.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swas.BAL.Interfaces
{
    public interface IProjStakeHolderCcMovRepository : IGenericRepositoryDL<tbl_ProjStakeHolderCcMov>
    {
        Task<tbl_ProjStakeHolderCcMov> GetdataBuPsmiandTounitId(int psmId, int TounitId);
    }
}
