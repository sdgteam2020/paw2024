using swas.BAL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swas.BAL.Interfaces
{
    public interface ICertificateService
    {
        CertificateDataDTO GetCertificateData(int projId, int substage);
    }
}
