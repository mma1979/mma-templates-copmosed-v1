using WebApplication1.Core.Models.Identity;

namespace WebApplication1.Core.Database.Identity;

 public partial class AppRefreshToken : BaseEntity<Guid>
    {
        public Guid UserId { get; private set; } // Linked to the AspNet Identity User Id
        public string Token { get; private set; }
        public string JwtId { get; private set; } // Map the token with jwtId
        public bool IsUsed { get; private set; } // if its used we dont want generate a new Jwt token with the same refresh token
        public bool IsRevoked { get; private set; } // if it has been revoke for security reasons
        public DateTime ExpiryDate { get; private set; } // Refresh token is long lived it could last for months.
        public int Hash { get; private set; } // Refresh token is long lived it could last for months.

        public virtual AppUser AppUser { get; private set; }

        #region Actions
        private AppRefreshToken()
        {

        }

        public AppRefreshToken(AppRefreshTokenModel dto)
        {
            UserId = dto.UserId;
            Token = dto.Token;
            JwtId = dto.JwtId;
            IsUsed = dto.IsUsed;
            IsRevoked = dto.IsRevoked;
            CreatedDate = dto.CreatedDate;
            ExpiryDate = dto.ExpiryDate;
            Hash = GetHashCode();
        }


        public AppRefreshToken Update(AppRefreshTokenModel dto)
        {
            UserId = dto.UserId;
            Token = dto.Token;
            JwtId = dto.JwtId;
            IsUsed = dto.IsUsed;
            IsRevoked = dto.IsRevoked;
            ModifiedDate = dto.CreatedDate;
            ExpiryDate = dto.ExpiryDate;
            Hash = GetHashCode();
            return this;
        }

        public AppRefreshToken MarkAsUsed()
        {
            IsUsed = true;
            ModifiedDate = DateTime.UtcNow;
            Hash = GetHashCode();

            return this;

        }

        public override int GetHashCode()
        {
            return HashCode.Combine(UserId, Token, JwtId, IsUsed, IsRevoked, CreatedDate, ExpiryDate);
        }

        public override bool Equals(object obj)
        {
            return obj is AppRefreshToken other &&
                UserId == other.UserId &&
                Token == other.Token &&
                JwtId == other.JwtId &&
                IsUsed == other.IsUsed &&
                IsRevoked == other.IsRevoked &&
                CreatedDate == other.CreatedDate &&
                ExpiryDate == other.ExpiryDate;
        }
        #endregion

    }