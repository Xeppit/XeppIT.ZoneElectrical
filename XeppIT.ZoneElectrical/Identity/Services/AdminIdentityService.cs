using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Driver;

namespace XeppIT.ZoneElectrical.Identity.Services
{
    public class AdminIdentityService
    {
        public readonly UserManager<ApplicationUser> UserManager;
        private readonly IMongoCollection<ApplicationUser> _applicationUserCollection;

        public AdminIdentityService(
            UserManager<ApplicationUser> userManager, IMongoCollection<ApplicationUser> applicationUserCollection)
        {
            UserManager = userManager;
            _applicationUserCollection = applicationUserCollection;
        }

        public event Action OnApplicationUsersChange;
        private void NotifyApplicationUsersStateChanged() => OnApplicationUsersChange?.Invoke();

        public async Task CreateUser(ApplicationUser user)
        {
            try
            {
                // Create the user
                // Result is reused after create user as the result for create role
                var result = await UserManager.CreateAsync(user, user.PasswordHash);

                // If create user failed send notification
                if (!result.Succeeded)
                {

                }
                else
                {
                    // If the user was created add to BasicAccess role


                    NotifyApplicationUsersStateChanged();
                }
            }
            catch (Exception ex)
            {

            }
        }

        public async Task<List<ApplicationUser>> GetUsers()
        {
            return await _applicationUserCollection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task<ApplicationUser> GetUserByUserName(string userName)
        {
            // Get user by name
            var user = await UserManager.FindByNameAsync(userName.ToUpper());

            if (user == null)
            {

            }

            return user;
        }

        public async Task<ApplicationUser> UpdateUser(ApplicationUser updatedUser)
        {

            // Update user roles
            try
            {
                var result = await UserManager.UpdateAsync(updatedUser);

                if (result.Succeeded)
                {
                    NotifyApplicationUsersStateChanged();

                    return updatedUser;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task ResetUserPassword(ApplicationUser user, string newPassword)
        {
            var resetToken = await UserManager.GeneratePasswordResetTokenAsync(user);

            try
            {
                var result = await UserManager.ResetPasswordAsync(user, resetToken, newPassword);
                
                if (result.Succeeded)
                {

                }
                else
                {

                }
            }
            catch (Exception ex)
            {

            }
        }

        public async Task DeleteUser(ApplicationUser user)
        {
            try
            {
                var result = await UserManager.DeleteAsync(user);
                if (result.Succeeded)
                {

                }
                else
                {

                }
            }
            catch (Exception ex)
            {

            }

        }
    }
}
