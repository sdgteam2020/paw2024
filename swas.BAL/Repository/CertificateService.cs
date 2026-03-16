using swas.BAL.DTO;
using swas.BAL.Interfaces;
using swas.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
			//var remotetestvalidationdate = substage == 29
			//	? _db.ProjStakeHolderMov
			//		.Where(x => x.ProjId == projId && x.StatusActionsMappingId == 78)
			//		.Select(x => (DateTime?)x.TimeStamp) // make nullable
			//		.FirstOrDefault() ?? DateTime.Now      // fallback to now if null
			//	: DateTime.Now;
			var remoteTestNext3Years = DateTime.Now.AddYears(3);
			var result = (from p in _db.Projects
						  join h in _db.mHostType
							  on p.HostTypeID equals h.HostTypeID into hostGrp
						  from h in hostGrp.DefaultIfEmpty() // LEFT JOIN
						  where p.ProjId == projId
						  select new CertificateDataDTO
						  {
							  ProjId = p.ProjId,
							  Sponsor = p.Sponsor,
							  ProjName = p.ProjName,
                              Security_Classification=p.Security_Classification,

                              RemoteTestNext3Years = remoteTestNext3Years, // new field for 3 years later
							  HostType = h != null ? h.HostingDesc : "N/A",
							  CertificateName = certName
						  }).FirstOrDefault();

			return result;
		}
	}
}

