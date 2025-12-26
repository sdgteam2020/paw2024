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
			// Get certificate name for the given substage
			var certName = _db.tbl_mCertificate
				.Where(x => x.Statusid == substage)
				.Select(x => x.CertificateName)
				.FirstOrDefault();

			// Get remote test validation date if substage is 29, else use current date
			var remotetestvalidationdate = substage == 29
				? _db.ProjStakeHolderMov
					.Where(x => x.ProjId == projId && x.StatusActionsMappingId == 78)
					.Select(x => (DateTime?)x.TimeStamp) // make nullable
					.FirstOrDefault() ?? DateTime.Now      // fallback to now if null
				: DateTime.Now;

			// Calculate date 3 years after the remote test clearance
			var remoteTestNext3Years = remotetestvalidationdate.AddYears(3);

			// Fetch project data
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

							  RemoteTestNext3Years = remoteTestNext3Years, // new field for 3 years later
							  HostType = h != null ? h.HostingDesc : "N/A",
							  CertificateName = certName
						  }).FirstOrDefault();

			return result;
		}
	}
}

