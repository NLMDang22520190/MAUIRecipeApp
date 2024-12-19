using MAUIRecipeApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MAUIRecipeApp.Service
{
    public class AuthService
    {
        private readonly RecipeDbContext _dbContext;

        public AuthService()
        {
            _dbContext = DataProvider.Ins.DB;
        }

        public async Task<User> AddOrUpdateUserAsync(string email, string username, string provider)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email && u.Provider == provider);

            if (user == null)
            {
                user = new User
                {
                    Username = username,
                    Email = email,
                    Provider = provider,
                    UserType = false,  // Hoặc loại khác nếu cần
                    Password = null  // OAuth không cần mật khẩu
                };

                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();
            }

            return user;
        }
    }
}
