using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;
using TaskManager.Infrastructure.Data;

namespace TaskManager.Infrastructure.Repositories
{
    public class StatusRepository : IStatusRepository
    {
        private readonly TaskManagerContext _context;

        public StatusRepository(TaskManagerContext context)
        {
            _context = context;
        }

        public async Task<Status> GetStatusByIdAsync(int id)
        {
            var status = await _context.Statuses.FirstOrDefaultAsync(x => x.StatusId == id);
            return status;
        }
    }
}
