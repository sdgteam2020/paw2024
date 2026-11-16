
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using swas.DAL;

namespace swas.UI.Helpers
{
    public class Appointmentservice
    {
        private readonly ApplicationDbContext _context;

        public Appointmentservice(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<List<SelectListItem>> LoadAppointments()
        {
            return await _context.MAppointments
                .Where(x => x.IsActive)
                .OrderBy(x => x.AppointmentName)
                .Select(x => new SelectListItem
                {
                    Value = x.ApptId.ToString(),
                    Text = x.AppointmentName
                })
                .ToListAsync();
        }
        public async Task<List<SelectListItem>> LoadSusNo()
        {
            return await _context.MSusNo
                .Where(x => x.IsActive)
                .OrderBy(x => x.SusNo)
                .Select(x => new SelectListItem
                {
                    Value = x.SusNo,
                    Text = x.SusNo
                })
                .ToListAsync();
        }
    }
}
