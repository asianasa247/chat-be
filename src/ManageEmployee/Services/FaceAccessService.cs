using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Entities;
using ManageEmployee.Services.Interfaces;
using System;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services
{
    public class FaceAccessService : IFaceAccessService
    {
        private readonly ApplicationDbContext _context;

        public FaceAccessService(ApplicationDbContext context)
        {
            _context = context;
        }

        public FaceAccess GetActiveAccess()
        {
            return _context.FaceAccesses.FirstOrDefault(x => x.IsActive);
        }

        public async Task<bool> AddAccessAsync(FaceAccess access)
        {
            try
            {
                access.Id = Guid.NewGuid();
                _context.FaceAccesses.Add(access);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<FaceAccess?> GetAccessByIdAsync(Guid accessId)
        {
            return await _context.FaceAccesses.FindAsync(accessId);
        }
        public async Task<bool> UpdateAccessAsync(FaceAccess access)
        {
            _context.FaceAccesses.Update(access);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<IEnumerable<FaceAccess>> GetAllAsync()
        {
            return await _context.FaceAccesses.ToListAsync();
        }
    }
}
