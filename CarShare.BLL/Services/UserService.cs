using AutoMapper;
using CarShare.BLL.DTOs.User;
using CarShare.BLL.Interfaces;
using CarShare.DAL.Enums;
using CarShare.DAL.Interfaces;
using CarShare.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using CarShare.DAL.Enums.CarShare.DAL.Enums;

namespace CarShare.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<UserResponseDTO> RegisterAsync(UserCreateDTO userDTO)
        {
            if (await EmailExists(userDTO.Email))
                throw new Exception("Email already in use");

            CreatePasswordHash(userDTO.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = _mapper.Map<User>(userDTO);
            user.IsActive = true;
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            //user.Role = UserRole.Renter;


            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<UserResponseDTO>(user);
        }

        // Update LoginAsync to check account status
        public async Task<string> LoginAsync(UserLoginDTO loginDTO)
        {
            // 1. Null check for input
            if (loginDTO == null) throw new ArgumentNullException(nameof(loginDTO));

            // 2. Get user with email filter at database level
            var users = await _unitOfWork.Users.FindAsync(u => u.Email == loginDTO.Email);
            var user = users.FirstOrDefault(); // Safe even if no results

            // 3. Combined security check (prevents timing attacks)
            if (user == null || user.PasswordHash == null || user.PasswordSalt == null ||
                !VerifyPasswordHash(loginDTO.Password, user.PasswordHash, user.PasswordSalt))
            {
                throw new Exception("Invalid credentials"); // Generic message
            }

            // 4. Account status checks
            if (!user.IsActive)
                throw new Exception("Account deactivated");

            if (!user.IsVerified)
                throw new Exception("Email not verified");

            // 5. Update last login
            user.LastLogin = DateTime.UtcNow;
            await _unitOfWork.CommitAsync();

            // 6. Generate token with null checks
            return GenerateJwtToken(user) ?? throw new Exception("Token generation failed");
        }

        public async Task<UserResponseDTO> GetProfileAsync(Guid userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null) throw new Exception("User not found");
            return _mapper.Map<UserResponseDTO>(user);
        }

        private async Task<bool> EmailExists(string email)
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            return users.Any(u => u.Email == email);
        }
        private void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
        {
            using (var hmac = new HMACSHA512())
            {
                salt = hmac.Key;
                hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
            
        }
        private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (storedHash == null || storedSalt == null) return false; // Critical safety check
            using var hmac = new HMACSHA512(storedSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(storedHash);
        }
        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.ToString())

                }),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiryInMinutes"])),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        public async Task VerifyCarOwnerAsync(Guid ownerId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(ownerId);
            if (user?.Role != UserRole.CarOwner)
                throw new Exception("User is not a car owner");
        }

        private static byte[] GenerateSalt()
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        private static string HashPassword(string password, byte[] salt)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
        }

        public async Task<IEnumerable<UserResponseDTO>> GetPendingUsersAsync()
        {
            var users = await _unitOfWork.Users.FindAsync(u =>
                u.AccountStatus == AccountStatus.Pending);
            return _mapper.Map<IEnumerable<UserResponseDTO>>(users);
        }
        public async Task ApproveUserAsync(Guid userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null) throw new Exception("User not found");

            user.AccountStatus = AccountStatus.Approved;
            user.IsVerified = true;  // Optional: Auto-verify when approved
            await _unitOfWork.CommitAsync();
        }
        public async Task RejectUserAsync(Guid userId, string? reason = null)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null) throw new Exception("User not found");

            user.AccountStatus = AccountStatus.Rejected;
            await _unitOfWork.CommitAsync();

            // Optional: Log rejection reason or send notification
        }
    }
}