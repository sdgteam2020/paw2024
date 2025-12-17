using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using swas.BAL.DTO;
using swas.BAL.Interfaces;
using swas.DAL;


namespace swas.BAL.Repository
{

    public class CertificateService : ICertificateService
    {
        private readonly ApplicationDbContext _db;

        public CertificateService(ApplicationDbContext db)
        {
            _db = db;
        }

        public CertificateDataDTO GetCertificateData(int projId, int substage)
        {
            var certName = _db.tbl_mCertificate
                .Where(x => x.Statusid == substage)
                .Select(x => x.CertificateName)
                .FirstOrDefault();

            return
                (from p in _db.Projects
                 join h in _db.mHostType
                     on p.HostTypeID equals h.HostTypeID into hostGrp
                 from h in hostGrp.DefaultIfEmpty() // LEFT JOIN
                 where p.ProjId == projId
                 select new CertificateDataDTO
                 {
                     ProjId = p.ProjId,
                     ProjName = p.ProjName,
                     HostType = h != null ? h.HostingDesc : "N/A",
                     CertificateName = certName
                 }).FirstOrDefault();
        }
    }
}
