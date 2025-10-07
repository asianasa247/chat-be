using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Services.Interfaces;
using System;
using ManageEmployee.Entities;
using Microsoft.EntityFrameworkCore;
namespace ManageEmployee.Services
{
    public class ScheduledPostService : IScheduledPostService
    {
        private readonly ApplicationDbContext _context;

        public ScheduledPostService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddScheduledPostAsync(ScheduledPost post)
        {
            _context.ScheduledPosts.Add(post);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ScheduledPost>> GetAllScheduledPostsAsync()
        {
            return await _context.ScheduledPosts.ToListAsync();
        }

        public async Task<ScheduledPost> GetScheduledPostByIdAsync(Guid id)
        {
            return await _context.ScheduledPosts.FindAsync(id);
        }

        public async Task UpdateScheduledPostAsync(ScheduledPost post)
        {
            _context.ScheduledPosts.Update(post);
            await _context.SaveChangesAsync();
        }
    }
}
