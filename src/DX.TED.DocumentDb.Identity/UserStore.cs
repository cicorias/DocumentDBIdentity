using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

//todo: use https://github.com/tracker086/DocumentDB.AspNet.Identity/blob/master/src/UserStore.cs 

namespace DX.TED.DocumentDb.Identity
{
    /// <summary>
    /// Class that implements the key ASP.NET Identity user store iterfaces
    /// </summary>
    public class UserStore<TUser> : IUserLoginStore<TUser>,
        IUserClaimStore<TUser>,
        //IUserRoleStore<TUser>,  //not using roles - this is really going to be a claim
        IUserPasswordStore<TUser>,
        IUserSecurityStampStore<TUser>,
        IQueryableUserStore<TUser>,
        IUserEmailStore<TUser>,
        IUserPhoneNumberStore<TUser>,
        IUserTwoFactorStore<TUser, string>,
        IUserLockoutStore<TUser, string>,
        IUserStore<TUser>,
        IDisposable
        where TUser : IdentityUser
    {

        UserCollection<TUser> _userCollection;
        bool _isDisposed = false;

        public DocumentDbContext DocumentDbContext { get; private set; }

        public IQueryable<TUser> Users
        {
            get
            {
                throw new NotImplementedException();
            }
        }


        /// <summary>
        /// Default constructor that initializes a new MySQLDatabase
        /// instance using the Default Connection string
        /// </summary>
        public UserStore()
        {
            new UserStore<TUser>(new DocumentDbContext());
        }

        /// <summary>
        /// Constructor that takes a MySQLDatabase as argument 
        /// </summary>
        /// <param name="database"></param>
        public UserStore(DocumentDbContext context)
        {
            this.DocumentDbContext = context;
            _userCollection = new UserCollection<TUser>(context);
        }

        /// <summary>
        /// Insert a new TUser in the UserTable
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task CreateAsync(TUser user)
        {
            ThrowIfDisposed();
            if (null == user)
                throw new ArgumentNullException("user");

            await _userCollection.Insert(user);

            //Task.FromResult<object>(_userCollection.Insert(user));
        }

        /// <summary>
        /// Returns an TUser instance based on a userId query 
        /// </summary>
        /// <param name="userId">The user's Id</param>
        /// <returns></returns>
        public Task<TUser> FindByIdAsync(string userId)
        {
            ThrowIfDisposed();
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("Null or empty argument: userId");

            TUser result = _userCollection.GetUserById(userId);
            if (result != null)
            {
                return Task.FromResult<TUser>(result);
            }

            return Task.FromResult<TUser>(null);
        }

        /// <summary>
        /// Returns an TUser instance based on a userName query 
        /// </summary>
        /// <param name="userName">The user's name</param>
        /// <returns></returns>
        public Task<TUser> FindByNameAsync(string userName)
        {
            ThrowIfDisposed();
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentException("Null or empty argument: userName");

            List<TUser> result = _userCollection.GetUserByName(userName);

            // Should I throw if > 1 user?
            if (result != null && result.Count == 1)
            {
                return Task.FromResult<TUser>(result[0]);
            }

            return Task.FromResult<TUser>(null);
        }

        /// <summary>
        /// Updates the UsersTable with the TUser instance values
        /// </summary>
        /// <param name="user">TUser to be updated</param>
        /// <returns></returns>
        public Task UpdateAsync(TUser user)
        {
            ThrowIfDisposed();
            if (null == user)
                throw new ArgumentNullException("user");

            _userCollection.Update(user);

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Inserts a claim to the UserClaimsTable for the given user
        /// </summary>
        /// <param name="user">User to have claim added</param>
        /// <param name="claim">Claim to be added</param>
        /// <returns></returns>
        public Task AddClaimAsync(TUser user, Claim claim)
        {
            ThrowIfDisposed();
            if (null == user)
                throw new ArgumentNullException("user");

            if (claim == null)
            {
                throw new ArgumentNullException("user");
            }

            _userCollection.InsertClaim(user, claim);

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Returns all claims for a given user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<IList<Claim>> GetClaimsAsync(TUser user)
        {
            ThrowIfDisposed();
            if (null == user)
                throw new ArgumentNullException("user");

            var rv = _userCollection.FindClaimsIdentityByUser(user);

            return Task.FromResult<IList<Claim>>(rv);
        }

        /// <summary>
        /// Removes a claim froma user
        /// </summary>
        /// <param name="user">User to have claim removed</param>
        /// <param name="claim">Claim to be removed</param>
        /// <returns></returns>
        public Task RemoveClaimAsync(TUser user, Claim claim)
        {
            ThrowIfDisposed();
            if (null == user)
                throw new ArgumentNullException("user");

            if (claim == null)
                throw new ArgumentNullException("claim");

            _userCollection.DeleteClaims(user, claim);

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Inserts a Login in the UserLoginsTable for a given User
        /// </summary>
        /// <param name="user">User to have login added</param>
        /// <param name="login">Login to be added</param>
        /// <returns></returns>
        public Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            ThrowIfDisposed();
            if (null == user)
                throw new ArgumentNullException("user");

            if (login == null)
                throw new ArgumentNullException("login");

            _userCollection.InsertLogin(user, login);

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Returns an TUser based on the Login info
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public Task<TUser> FindAsync(UserLoginInfo login)
        {
            ThrowIfDisposed();
            if (null == login)
                throw new ArgumentNullException("login");

            var user = _userCollection.FindUserIdByLogin(login);
            if (user != null)
            {
                return Task.FromResult<TUser>(user);
            }

            return Task.FromResult<TUser>(null);
        }

        /// <summary>
        /// Returns list of UserLoginInfo for a given TUser
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            ThrowIfDisposed();
            if (null == user)
                throw new ArgumentNullException("user");

            List<UserLoginInfo> userLogins = new List<UserLoginInfo>();

            List<UserLoginInfo> logins = _userCollection.FindLoginsByUser(user);
            if (logins != null)
            {
                return Task.FromResult<IList<UserLoginInfo>>(logins);
            }

            return Task.FromResult<IList<UserLoginInfo>>(null);
        }

        /// <summary>
        /// Deletes a login from UserLoginsTable for a given TUser
        /// </summary>
        /// <param name="user">User to have login removed</param>
        /// <param name="login">Login to be removed</param>
        /// <returns></returns>
        public Task RemoveLoginAsync(TUser user, UserLoginInfo login)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            _userCollection.DeleteUserLogin(user, login);

            return Task.FromResult<Object>(null);
        }


        /// <summary>
        /// Deletes a user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task DeleteAsync(TUser user)
        {
            ThrowIfDisposed();
            if (null == user)
                throw new ArgumentNullException("user");

            _userCollection.Delete(user);

            return Task.FromResult<Object>(null);
        }

        /// <summary>
        /// Returns the PasswordHash for a given TUser
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<string> GetPasswordHashAsync(TUser user)
        {
            ThrowIfDisposed();
            if (null == user)
                throw new ArgumentNullException("user");

            return Task.FromResult<string>(user.PasswordHash);
        }

        /// <summary>
        /// Verifies if user has password
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> HasPasswordAsync(TUser user)
        {
            ThrowIfDisposed();
            if (null == user)
                throw new ArgumentNullException("user");

            var hasPassword = !string.IsNullOrEmpty(_userCollection.GetPasswordHash(user));

            return Task.FromResult<bool>(Boolean.Parse(hasPassword.ToString()));
        }

        /// <summary>
        /// Sets the password hash for a given TUser
        /// </summary>
        /// <param name="user"></param>
        /// <param name="passwordHash"></param>
        /// <returns></returns>
        public Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            ThrowIfDisposed();
            if (null == user)
                throw new ArgumentNullException("user");

            user.PasswordHash = passwordHash;

            return Task.FromResult<Object>(null);
        }

        /// <summary>
        ///  Set security stamp
        /// </summary>
        /// <param name="user"></param>
        /// <param name="stamp"></param>
        /// <returns></returns>
        public Task SetSecurityStampAsync(TUser user, string stamp)
        {
            ThrowIfDisposed();
            if (null == user)
                throw new ArgumentNullException("user");

            user.SecurityStamp = stamp;

            return Task.FromResult<int>(0);

        }

        /// <summary>
        /// Get security stamp
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<string> GetSecurityStampAsync(TUser user)
        {
            ThrowIfDisposed();
            if (null == user)
                throw new ArgumentNullException("user");

            return Task.FromResult<string>(user.SecurityStamp);
        }

        /// <summary>
        /// Set email on user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public Task SetEmailAsync(TUser user, string email)
        {
            ThrowIfDisposed();
            if (null == user)
                throw new ArgumentNullException("user");

            user.Email = email;

            return Task.FromResult<int>(0);

        }

        /// <summary>
        /// Get email from user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<string> GetEmailAsync(TUser user)
        {
            ThrowIfDisposed();
            if (null == user)
                throw new ArgumentNullException("user");

            return Task.FromResult<string>(user.Email);
        }

        /// <summary>
        /// Get if user email is confirmed
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> GetEmailConfirmedAsync(TUser user)
        {
            ThrowIfDisposed();
            if (null == user)
                throw new ArgumentNullException("user");

            return Task.FromResult<bool>(user.EmailConfirmed);
        }

        /// <summary>
        /// Set when user email is confirmed
        /// </summary>
        /// <param name="user"></param>
        /// <param name="confirmed"></param>
        /// <returns></returns>
        public Task SetEmailConfirmedAsync(TUser user, bool confirmed)
        {
            ThrowIfDisposed();
            if (null == user)
                throw new ArgumentNullException("user");

            user.EmailConfirmed = confirmed;
            _userCollection.Update(user);

            return Task.FromResult(0);
        }

        /// <summary>
        /// Get user by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public Task<TUser> FindByEmailAsync(string email)
        {
            if (String.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException("email");
            }

            TUser result = _userCollection.GetUserByEmail(email) as TUser;
            if (result != null)
            {
                return Task.FromResult<TUser>(result);
            }

            return Task.FromResult<TUser>(null);
        }

        /// <summary>
        /// Set user phone number
        /// </summary>
        /// <param name="user"></param>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public Task SetPhoneNumberAsync(TUser user, string phoneNumber)
        {
            ThrowIfDisposed();
            if (null == user)
                throw new ArgumentNullException("user");

            user.PhoneNumber = phoneNumber;

            return Task.FromResult<int>(0);
        }

        /// <summary>
        /// Get user phone number
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<string> GetPhoneNumberAsync(TUser user)
        {
            ThrowIfDisposed();
            if (null == user)
                throw new ArgumentNullException("user");

            return Task.FromResult<string>(user.PhoneNumber);
        }

        /// <summary>
        /// Get if user phone number is confirmed
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> GetPhoneNumberConfirmedAsync(TUser user)
        {
            ThrowIfDisposed();
            if (null == user)
                throw new ArgumentNullException("user");

            return Task.FromResult<bool>(user.PhoneNumberConfirmed);
        }

        /// <summary>
        /// Set phone number if confirmed
        /// </summary>
        /// <param name="user"></param>
        /// <param name="confirmed"></param>
        /// <returns></returns>
        public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed)
        {
            ThrowIfDisposed();
            if (null == user)
                throw new ArgumentNullException("user");

            user.PhoneNumberConfirmed = confirmed;

            return Task.FromResult<int>(0);
        }

        /// <summary>
        /// Set two factor authentication is enabled on the user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
        {
            ThrowIfDisposed();
            if (null == user)
                throw new ArgumentNullException("user");

            user.TwoFactorEnabled = enabled;

            return Task.FromResult<int>(0);
        }

        /// <summary>
        /// Get if two factor authentication is enabled on the user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> GetTwoFactorEnabledAsync(TUser user)
        {
            ThrowIfDisposed();
            if (null == user)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.TwoFactorEnabled);
        }

        /// <summary>
        /// Get user lock out end date
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
        {
            ThrowIfDisposed();
            if (null == user)
                throw new ArgumentNullException("user");

            return
                Task.FromResult(user.LockoutEndDateUtc.HasValue
                    ? new DateTimeOffset(DateTime.SpecifyKind(user.LockoutEndDateUtc.Value, DateTimeKind.Utc))
                    : new DateTimeOffset());
        }


        /// <summary>
        /// Set user lockout end date
        /// </summary>
        /// <param name="user"></param>
        /// <param name="lockoutEnd"></param>
        /// <returns></returns>
        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
        {
            ThrowIfDisposed();
            if (null == user)
                throw new ArgumentNullException("user");

            user.LockoutEndDateUtc = lockoutEnd.UtcDateTime;

            return Task.FromResult<int>(0);
        }

        /// <summary>
        /// Increment failed access count
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<int> IncrementAccessFailedCountAsync(TUser user)
        {
            ThrowIfDisposed();
            if (null == user)
                throw new ArgumentNullException("user");

            user.AccessFailedCount++;

            return Task.FromResult<int>(user.AccessFailedCount);
        }

        /// <summary>
        /// Reset failed access count
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task ResetAccessFailedCountAsync(TUser user)
        {
            ThrowIfDisposed();
            if (null == user)
                throw new ArgumentNullException("user");

            user.AccessFailedCount = 0;

            return Task.FromResult<int>(0);
        }

        /// <summary>
        /// Get failed access count
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<int> GetAccessFailedCountAsync(TUser user)
        {
            ThrowIfDisposed();
            if (null == user)
                throw new ArgumentNullException("user");

            return Task.FromResult<int>(user.AccessFailedCount);
        }

        /// <summary>
        /// Get if lockout is enabled for the user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> GetLockoutEnabledAsync(TUser user)
        {
            ThrowIfDisposed();
            if (null == user)
                throw new ArgumentNullException("user");

            return Task.FromResult<bool>(user.LockoutEnabled);
        }

        /// <summary>
        /// Set lockout enabled for user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public Task SetLockoutEnabledAsync(TUser user, bool enabled)
        {
            ThrowIfDisposed();
            if (null == user)
                throw new ArgumentNullException("user");

            user.LockoutEnabled = enabled;

            return Task.FromResult<int>(0);
        }

        public void Dispose()
        {
            /// TODO: proper dispose implmentation
            if (!this._isDisposed)
            {
                this.DocumentDbContext = null;
                this._isDisposed = true;
            }
        }

        void ThrowIfDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }
        }

    }

}
