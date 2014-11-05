using Microsoft.AspNet.Identity;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

//using DX.TED.DocumentDb.Identity.

namespace DX.TED.DocumentDb.Identity
{
    public class UserCollection<TUser> : IDisposable
        where TUser : IdentityUser
    {
        DocumentDbContext _context;
        bool _isDisposed = false;
        DocumentClient _client = null;

        public UserCollection(DocumentDbContext context)
        {
            if (null == context)
                throw new ArgumentNullException("context");

            this._context = context;
            this._client = context.Client;
        }


        internal async virtual Task<string> Insert(TUser user)
        {
            ThrowIfDisposed();
            if (null == user)
                throw new ArgumentNullException("user");

            var tUser = await _client.CreateDocumentAsync(_context.UserCollection.DocumentsLink, user);

            return tUser.Resource.Id;
        }

        internal TUser GetUserById(string userId)
        {
            ThrowIfDisposed();
            if (null == userId)
                throw new ArgumentNullException("userId");

            var result = this._client.CreateDocumentQuery<TUser>(this._context.UserCollection.SelfLink)
                     .Where(f => f.Id == userId).ToArray().FirstOrDefault();

            return result;
        }

        internal List<TUser> GetUserByName(string userName)
        {
            ThrowIfDisposed();
            if (null == userName)
                throw new ArgumentNullException("userId");

            var result = this._client.CreateDocumentQuery<TUser>(this._context.UserCollection.SelfLink)
                     .Where(f => f.UserName == userName);//.ToList();

            if (null != result)
                return result.ToList();

            return null;// return result;
        }

        internal void Update(TUser user)
        {
            throw new NotImplementedException();
        }


        /// Claims Stuff


        internal void InsertClaim(TUser user, Claim claim)
        {
            throw new NotImplementedException();
        }

        internal IList<Claim> FindClaimsIdentityByUser(TUser user)
        {
            var claims = from c in user.Claims
                     select new Claim(c.ClaimType, c.ClaimValue);

            return claims.ToList<Claim>();

        }

        internal void DeleteClaims(TUser user, Claim claim)
        {
            throw new NotImplementedException();
        }

        /// Logins stuff
        internal void InsertLogin(TUser user, UserLoginInfo login)
        {
            throw new NotImplementedException();
        }

        internal TUser FindUserIdByLogin(UserLoginInfo login)
        {
            throw new NotImplementedException();
        }

        internal List<UserLoginInfo> FindLoginsByUser(TUser user)
        {
            throw new NotImplementedException();
        }

        internal void DeleteUserLogin(TUser user, UserLoginInfo login)
        {
            throw new NotImplementedException();
        }



        /// User Stuff
        internal void Delete(TUser user)
        {
            throw new NotImplementedException();
        }

        internal string GetPasswordHash(TUser user)
        {
            throw new NotImplementedException();
        }

        internal TUser GetUserByEmail(string email)
        {
            ThrowIfDisposed();
            if (null == email)
                throw new ArgumentNullException("userId");

            var result = this._client.CreateDocumentQuery<TUser>(this._context.UserCollection.SelfLink)
                     .Where(f => f.Email == email);

            if (null != result)
                return result.ToArray().FirstOrDefault();

            return null;
        }

        public void Dispose()
        {
            /// TODO: proper dispose implmentation
            if (!this._isDisposed)
            {
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
