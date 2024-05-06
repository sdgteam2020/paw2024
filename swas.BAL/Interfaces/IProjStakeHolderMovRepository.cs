﻿using swas.BAL.DTO;
using swas.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swas.BAL.Interfaces
{

    ///Created and Reviewed by : Sub Maj Sanal
    ///Reviewed Date : 31 Jul 23
    ///Tested By :- 
    ///Tested Date : 
    ///Start
    public interface IProjStakeHolderMovRepository
    {
        Task<int> AddProjStakeHolderMovAsync(Projmove psmove);
        Task<tbl_ProjStakeHolderMov> GetProjStakeHolderMovByIdAsync(int psmId);
        Task<List<tbl_ProjStakeHolderMov>> GetAllProjStakeHolderMovAsync();
        Task<bool> UpdateProjStakeHolderMovAsync(tbl_ProjStakeHolderMov projStakeHolderMov);
        Task<bool> DeleteProjStakeHolderMovAsync(int psmId);
        Task<int> CountinboxAsync(int stkhol);
       

        Task<int> AddProStkMovBlogAsync(Projmove psmove);
        Task<int> ValStatusAsync(int? ProjId);
        Task<int> GetlaststageId(int? ProjId);
        Task<int> ReturnDuplProjMovAsync(Projmove psmove);
        Task<int> RetWithObsnMovAsync(Projmove psmove);
        Task<List<ProjLogView>> GetProjLogviewAsync(string startDate, string endDate);
    }


}
