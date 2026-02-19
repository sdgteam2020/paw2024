using Grpc.Core;
using swas.BAL.DTO;
using swas.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swas.BAL.Interfaces
{
    public interface IUnitRepository
    { 
        Task<int> Save(UnitDtl Db);
        bool CheckUserExist(string UserName); 
        Task<int> del(UnitDtl Db);
        Task<List<UnitDtl>> GetAllUnitAsync();

        Task<List<DTOUnitMapping>> GetallUnitwithmap();

        Task<List<DTOUnitMapping>> GetallUnitwithmap1(int StageId ,int StatusId);


        Task<List<UnitDtl>> GetAllStakeHolderComment();


        
        Task<UnitDtl> GetUnitDtl(int unitid);
        Task<int> GetIdCalendar();

        Task<UnitDtl> GetUnitDtlwithname(string UnitName);
        Task<UnitDtl> GetUnitDtls(int unitid);
        Task<List<DTODDLComman>> GetAllUnitNotDte();


    }
}
