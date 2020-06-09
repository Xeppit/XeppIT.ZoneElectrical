using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Driver;
using Radzen;

namespace XeppIT.ZoneElectrical.Identity.Services
{
    public class AdminIdentityService
    {
        public readonly UserManager<ApplicationUser> UserManager;
        private readonly IMongoCollection<ApplicationUser> _applicationUserCollection;
        private readonly NotificationService _notificationService;

        public AdminIdentityService(
            UserManager<ApplicationUser> userManager, IMongoCollection<ApplicationUser> applicationUserCollection, NotificationService notificationService)
        {
            UserManager = userManager;
            _applicationUserCollection = applicationUserCollection;
            _notificationService = notificationService;
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
                    var notificationMessages = new NotificationMessage()
                    { Severity = NotificationSeverity.Error, Summary = "User Not Create" };

                    foreach (var error in result.Errors)
                    {
                        notificationMessages.Detail = $"{notificationMessages.Detail + error.Description + Environment.NewLine}";
                    }

                    _notificationService.Notify(notificationMessages);
                }
                else
                {
                    // If the user was created add to BasicAccess role
                    var notificationMessages = new NotificationMessage()
                        { Severity = NotificationSeverity.Success, Summary = "User Created", Detail = $"User with the username {user.UserName} has been create" };

                    _notificationService.Notify(notificationMessages);

                    NotifyApplicationUsersStateChanged();
                }
            }
            catch (Exception ex)
            {
                var notificationMessages = new NotificationMessage()
                { Severity = NotificationSeverity.Error, Summary = "Exception", Detail = ex.Message };

                _notificationService.Notify(notificationMessages);
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
                var notificationMessages = new NotificationMessage()
                { Severity = NotificationSeverity.Success, Summary = "User Not Found", Detail = $"User with the username {userName} could not be found" };

                _notificationService.Notify(notificationMessages);

                return null;
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
                    var notificationMessages = new NotificationMessage()
                        { Severity = NotificationSeverity.Success, Summary = "User updated", Detail = $"User with the username {updatedUser.UserName} has been updated" };

                    _notificationService.Notify(notificationMessages);

                    NotifyApplicationUsersStateChanged();

                    return updatedUser;
                }
                else
                {
                    var notificationMessages = new NotificationMessage()
                    { Severity = NotificationSeverity.Error, Summary = "User not updated" };

                    foreach (var error in result.Errors)
                    {
                        notificationMessages.Detail = $"{notificationMessages.Detail + error.Description + Environment.NewLine}";
                    }

                    _notificationService.Notify(notificationMessages);

                    return null;
                }

            }
            catch (Exception ex)
            {
                var notificationMessages = new NotificationMessage()
                { Severity = NotificationSeverity.Success, Summary = "Exception", Detail = $"{ex.Message}" };

                _notificationService.Notify(notificationMessages);

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
                    var notificationMessages = new NotificationMessage()
                    { Severity = NotificationSeverity.Success, Summary = "Password reset", Detail = $"User with the username {user.UserName} password has been updated" };

                    _notificationService.Notify(notificationMessages);
                }
                else
                {
                    var notificationMessages = new NotificationMessage()
                    { Severity = NotificationSeverity.Error, Summary = "User not password not reset" };

                    foreach (var error in result.Errors)
                    {
                        notificationMessages.Detail = $"{notificationMessages.Detail + error.Description + Environment.NewLine}";
                    }

                    _notificationService.Notify(notificationMessages);
                }
            }
            catch (Exception ex)
            {

                var notificationMessages = new NotificationMessage()
                { Severity = NotificationSeverity.Success, Summary = "Exception", Detail = $"{ex.Message}" };

                _notificationService.Notify(notificationMessages);
            }
        }

        public async Task DeleteUser(ApplicationUser user)
        {
            try
            {
                var result = await UserManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    var notificationMessages = new NotificationMessage()
                    { Severity = NotificationSeverity.Success, Summary = "User deleted", Detail = $"User with the username {user.UserName} has been deleted" };

                    _notificationService.Notify(notificationMessages);

                    NotifyApplicationUsersStateChanged();
                }
                else
                {
                    var notificationMessages = new NotificationMessage()
                    { Severity = NotificationSeverity.Error, Summary = "User not deleted" };

                    foreach (var error in result.Errors)
                    {
                        notificationMessages.Detail = $"{notificationMessages.Detail + error.Description + Environment.NewLine}";
                    }

                    _notificationService.Notify(notificationMessages);
                }
            }
            catch (Exception ex)
            {

                var notificationMessages = new NotificationMessage()
                { Severity = NotificationSeverity.Success, Summary = "Exception", Detail = $"{ex.Message}" };

                _notificationService.Notify(notificationMessages);
            }

        }
    }
}
