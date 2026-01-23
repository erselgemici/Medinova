using Medinova.DTOs.AccountDtos;
using Medinova.Models;
using System.Linq;

namespace Medinova.Repositories.UserRepositories
{
    public class UserRepository
    {
        MedinovaContext db = new MedinovaContext();

        // Geriye 'User' nesnesi döndürüyoruz ki Controller session'a atabilsin.
        public User ValidateUser(LoginDto loginDto)
        {
            var user = db.Users.Include("Roles")
                                           .FirstOrDefault(x => x.Email == loginDto.Email && x.Password == loginDto.Password);
            if (user != null)
            {
                bool isAdmin = user.Roles.Any(r => r.RoleName == "Admin");

                if (isAdmin)
                {
                    return user;
                }
            }

            return null;
        }
    }
}
