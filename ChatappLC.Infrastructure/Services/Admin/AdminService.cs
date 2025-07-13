using ChatappLC.Application.DTOs.Admin;
using ChatappLC.Application.DTOs.User;
using ChatappLC.Application.Interfaces.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatappLC.Infrastructure.Services.Admin
{
    public class AdminService : IAdminService
    {
        private readonly MongoDbContext _context;

        public AdminService(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserResponse>> GetAllUsersAsync()
        {
            var users = await _context.Users.Find(_ => true).ToListAsync();
            return users.Select(u => new UserResponse
            {
                Id = u.Id,
                Email = u.Email,
                Username = u.Username,
                PhoneNumber = u.PhoneNumber,
                Role = u.Role
            }).ToList();
        }

        public async Task<UserResponse?> GetUserByIdAsync(string id)
        {
            var user = await _context.Users.Find(u => u.Id == id).FirstOrDefaultAsync();
            if (user == null) return null;

            return new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Username,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role
            };
        }

        public async Task<bool> UpdateUserAsync(UserUpdateRequest request)
        {
            var update = Builders<User>.Update
                .Set(u => u.Email, request.Email)
                .Set(u => u.Username, request.Username)
                .Set(u => u.PhoneNumber, request.PhoneNumber)
                .Set(u => u.FullName, request.FullName)
                .Set(u => u.Role, request.Role);

            var result = await _context.Users.UpdateOneAsync(
                u => u.Id == request.Id,
                update
            );

            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            var result = await _context.Users.DeleteOneAsync(u => u.Id == id);
            return result.DeletedCount > 0;
        }
    }

}
