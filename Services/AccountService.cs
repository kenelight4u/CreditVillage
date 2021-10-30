using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CreditVillageBackend.Helpers;
using CreditVillageBackend.Interfaces;
using CreditVillageBackend.ViewModels.RequestViewModels;
using CreditVillageBackend.ViewModels.ResponseViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Vonage;
using Vonage.Request;

namespace CreditVillageBackend
{
    public class AccountService : IAccount
    {
        private readonly UserManager<AppUser> _userManager;

        private readonly AppSettings _appSettings;

        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly IMailService _mailService;

        private readonly IUploadImage _uploadImage;

        public AccountService(UserManager<AppUser> userManager,
                                IOptions<AppSettings> appSettings,
                                RoleManager<IdentityRole> roleManager,
                                IMailService mailService,
                                IUploadImage uploadImage)
        {
            _userManager = userManager;
            _appSettings = appSettings.Value;
            _roleManager = roleManager;
            _mailService = mailService;
            _uploadImage = uploadImage;
        }

        public async Task<RegisterResponse> UserRegisterAsync(RegisterRequest userRequest)
        {
            var existingEmail = await _userManager.FindByEmailAsync(userRequest.Email);

            if (existingEmail != null)
            {
                return new RegisterResponse()
                {
                    Check = false,
                    Status = "error",
                    Message = "This Email Address is Already Used"
                };
            }

            var newUser = new AppUser
            {
                Email = userRequest.Email,
                UserName = userRequest.Email,
                CreatedOn = DateTime.Now,
            };

            var result = await _userManager.CreateAsync(newUser, userRequest.Password);

            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync(UserRoles.User))
                    await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));

                if (await _roleManager.RoleExistsAsync(UserRoles.User))
                {
                    await _userManager.AddToRoleAsync(newUser, UserRoles.User);
                }

                string code = await _userManager.GenerateTwoFactorTokenAsync(newUser, "Email");

                await SendCodeToUser(newUser.Email, code);

                return new RegisterResponse()
                {
                    Check = true,
                    Status = "success",
                    Message = $"Account Created Successfully. Please Check Your Email For OTP",
                    UserId = newUser.Id,
                    Email = newUser.Email,
                    Code = code
                };
            }

            return new RegisterResponse()
            {
                Check = false,
                Status = "error",
                Message = result.Errors.LastOrDefault().Description
            };
        }

        
        public async Task<RegisterResponse> AdminRegisterAsync(RegisterRequest userRequest)
        {
            var existingEmail = await _userManager.FindByEmailAsync(userRequest.Email);

            if (existingEmail != null)
            {
                return new RegisterResponse()
                {
                    Check = false,
                    Status = "error",
                    Message = "This Email Address is Already Used"
                };
            }

            var newUser = new AppUser
            {
                Email = userRequest.Email,
                UserName = userRequest.Email,
                CreatedOn = DateTime.Now,
            };

            var result = await _userManager.CreateAsync(newUser, userRequest.Password);

            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                    await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));

                if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
                {
                    await _userManager.AddToRoleAsync(newUser, UserRoles.Admin);
                }

                string code = await _userManager.GenerateTwoFactorTokenAsync(newUser, "Email");

                await SendCodeToUser(newUser.Email, code);

                return new RegisterResponse()
                {
                    Check = true,
                    Status = "success",
                    Message = $"Account Created Successfully. Please Check Your Email For OTP",
                    UserId = newUser.Id,
                    Email = newUser.Email,
                    Code = code
                };
            }

            return new RegisterResponse()
            {
                Check = false,
                Message = result.Errors.LastOrDefault().Description
            };
        }


        public async Task<ResendCodeResponse> UserResendCodeAsync(ResendCodeRequest userRequest)
        {
            var existingUser = await _userManager.FindByEmailAsync(userRequest.Email);

            if (existingUser == null)
            {
                return new ResendCodeResponse()
                {
                    Check = false,
                    Status = "error",
                    Message = "Email doesn't Exist"
                };
            }

            string code = await _userManager.GenerateTwoFactorTokenAsync(existingUser, "Email" );

            await SendCodeToUser(existingUser.Email, code);

            return new ResendCodeResponse()
            {
                Check = true,
                Status = "success",
                Message = "Please Check Your Email For OTP"
            };
        }


        public async Task<VerifyResponse> UserVerifyAsync(VerifyRequest userRequest)
        {
            var existUser = await _userManager.FindByEmailAsync(userRequest.Email);

            if (existUser == null)
            {
                return new VerifyResponse()
                {
                    Check = false,
                    Status = "error",
                    Message = "This Email Address doesn't exist"
                };
            }

            bool result = await _userManager.VerifyTwoFactorTokenAsync(existUser, "Email", userRequest.Token);

            if (!result)
            {
                return new VerifyResponse()
                {
                    Check = false,
                    Status = "error",
                    Message = "Invalid token"
                };
            }
            else
            {
                existUser.EmailConfirmed = true;

                await _userManager.UpdateAsync(existUser);

                return new VerifyResponse()
                {
                    Check = true,
                    Status = "success",
                    Message = "Email Verified Successfully"
                };
            }
        }


        public async Task<LoginResponse> LoginAsync(LoginRequest userRequest)
        {
            var existUser = await _userManager.FindByNameAsync(userRequest.Email);

            var checkPassword = await _userManager.CheckPasswordAsync(existUser, userRequest.Password);

            if (existUser == null || !checkPassword)
            {
                return new LoginResponse()
                {
                    Check = false,
                    Status = "error",
                    Message = "Email or Password is Wrong"
                };
            }

            //var checkPassword = await _userManager.CheckPasswordAsync(existUser, userRequest.Password);

            //if (!checkPassword)
            //{
            //    return new LoginResponse()
            //    {
            //        Check = false,
            //        Status = "success",
            //        Message = "Wrong Password or Email"
            //    };
            //}

            var emailConfirmed = await _userManager.IsEmailConfirmedAsync(existUser);

            if (!emailConfirmed)
            {
                return new LoginResponse()
                {
                    Check = false,
                    Status = "error",
                    Message = "Your Email Address has not been confirm. Kindly Check Your Email Address"
                };
            }

            return await GenerateUserToken(existUser);
        }


        public async Task<ForgetPasswordResponse> UserForgetPasswordAsync(ForgetPasswordRequest userRequest)
        {
            var existingUser = await _userManager.FindByEmailAsync(userRequest.Email);

            if (existingUser == null)
            {
                return new ForgetPasswordResponse()
                {
                    Check = false,
                    Status = "error",
                    Message = "Email does not Exist, Please Register"
                };
            }
            if (!(await _userManager.IsEmailConfirmedAsync(existingUser)))
            {
                return new ForgetPasswordResponse()
                {
                    Check = false,
                    Status = "error",
                    Message = "Your Email Address has not been confirm. Kindly Check Your Email Address"
                };
            }

            string code = await _userManager.GeneratePasswordResetTokenAsync(existingUser);

            return new ForgetPasswordResponse()
            {
                Check = true,
                Status = "success",
                Message = $"Reset Password link has been sent to your mail",
                FirstName = existingUser.FirstName,
                UserId = existingUser.Id,
                Email = existingUser.Email,
                Code = code
            };
        }

        public async Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordRequest userRequest)
        {
            var existingUser = await _userManager.FindByEmailAsync(userRequest.Email);

            if (existingUser == null)
            {
                return new ResetPasswordResponse()
                {
                    Check = false,
                    Status = "error",
                    Message = "Email does not Exist, Please Register"
                };
            }
            if (!(await _userManager.IsEmailConfirmedAsync(existingUser)))
            {
                return new ResetPasswordResponse()
                {
                    Check = false,
                    Status = "error",
                    Message = "Your Email Address has not been confirm. Kindly Check Your Email Address"
                };
            }

            var result = await _userManager.ResetPasswordAsync(existingUser, userRequest.Code, userRequest.NewPassword);

            if (!result.Succeeded)
            {
                return new ResetPasswordResponse()
                {
                    Check = false,
                    Status = "error",
                    Message = "Invalid Token"
                };
            }

            return new ResetPasswordResponse()
            {
                Check = true,
                Status = "success",
                Message = "Password Reset Successfully"
            }; 
        }


        public async Task<ChangePasswordResponse> ChangePasswordAsync(string GetUserId, ChangePasswordRequest changePasswordRequest)
        {
            var user = await _userManager.FindByIdAsync(GetUserId);

            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                return new ChangePasswordResponse()
                {
                    Check = false,
                    Status = "error",
                    Message = "Your Email Address has not been confirm. Kindly Check Your Email Address"
                };
            }

            var result = await _userManager.ChangePasswordAsync(user, changePasswordRequest.Old_Password, changePasswordRequest.Password);

            if(!result.Succeeded)
            {
                return new ChangePasswordResponse()
                {
                    Check = false,
                    Status = "error",
                    Message = "Your Old Password is Incorrect"
                };
            }

            return new ChangePasswordResponse()
            {
                Check = true,
                Status = "success",
                Message = "Password Changed"
            };
        }

        public async Task<GetUserResponse> GetUserAsync(string UserId)
        {

            var newUser = await _userManager.Users.SingleOrDefaultAsync(s => s.Id == UserId);

            var fileDetails = await _uploadImage.GetFileFromDatabase(newUser.LogoFileId.ToString());

            if (fileDetails != null)
            {
                var getResponse = new GetUserResponse()
                {
                    FirstName = newUser.FirstName,
                    LastName = newUser.LastName,
                    FullName = newUser.FirstName + " " + newUser.LastName,
                    Gender = newUser.Gender,
                    Email = newUser.Email,
                    PhoneNumber = newUser.PhoneNumber,
                    FileBase64 = fileDetails.File,
                    FileFullName = fileDetails.FullName
                };

                return getResponse;
            }
            else
            {
                var getUserResponse = new GetUserResponse()
                {
                    FirstName = newUser.FirstName,
                    LastName = newUser.LastName,
                    FullName = newUser.FirstName + " " + newUser.LastName,
                    Gender = newUser.Gender,
                    Email = newUser.Email,
                    PhoneNumber = newUser.PhoneNumber
                };

                return getUserResponse;
            }
        }


        public async Task<UpdateResponse> UpdateUserAsync(string GetUserId, UpdateRequest updateRequest)
        {
            var user = await _userManager.FindByIdAsync(GetUserId);

            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                return new UpdateResponse()
                {
                    Check = false,
                    Status = "error",
                    Message = "Username Doesn't Exist"
                };
            }

            user.FirstName = updateRequest.FirstName;
            user.LastName = updateRequest.LastName;
            user.PhoneNumber = updateRequest.PhoneNumber;
            user.ModifiedOn = DateTime.Now;

            await _userManager.UpdateAsync(user);

            return new UpdateResponse()
            {
                Check = true,
                Status = "success",
                Message = "Updated Successfully"
            };
        }

        public async Task<EditResponse> EditUserDetailsAsync(string GetUserId, EditRequest editRequest)
        {
            var user = await _userManager.FindByIdAsync(GetUserId);

            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                return new EditResponse()
                {
                    Check = false,
                    Status = "error",
                    Message = "Username Doesn't Exist"
                };
            }

            if (editRequest.LogoFile != null)
            {
                var imageId = await _uploadImage.UploadImageToDatabase(editRequest.LogoFile, user.LogoFileId);
                user.LogoFileId = imageId;
            }
            
            user.FirstName = editRequest.FirstName;
            user.LastName = editRequest.LastName;
            user.PhoneNumber = editRequest.PhoneNumber;
            user.Gender = editRequest.Gender;
            user.ModifiedOn = DateTime.Now;
            
            await _userManager.UpdateAsync(user);

            return new EditResponse()
            {
                Check = true,
                Status = "success",
                Message = "Updated Successfully"
            };
        }

        private async Task SendCodeToUser(string userEmail, string code)
        {
            string content = 
                $"<h4>Credit Village account</h4>" +
                $"<p style='font-size: 41px; color: #2672ec'> Confirm your email address</p>" +
                $"<p>To finish setting up your Credit Village account, we just need to make sure this email address is yours.</p>" +
                $"<p>To verify your email address use this security code: <b>{ code }</b></p>" +
                $"<p>If you didn't request this code, you can safely ignore this email. Someone else might have typed your email address by mistake.</p>" +
                $"<p>Thanks,</p>" +
                $"<p>The Credit Village team</p>";

            var subject = $"Confirm your email address";
            
           await _mailService.SendEmailAsync(userEmail, subject, content);
        }

        private async Task<LoginResponse> GenerateUserToken(AppUser user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Secret));

            double tokenExpiryTime = Convert.ToDouble(_appSettings.ExpireTime);

            var Expires = DateTime.UtcNow.AddDays(tokenExpiryTime);

            var tokenHandler = new JwtSecurityTokenHandler();

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var roles = await _userManager.GetRolesAsync(user);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                //new Claim(ClaimTypes.GivenName, user.FirstName),
                //new Claim(ClaimTypes.Surname, user.LastName),
                new Claim(ClaimTypes.Role, roles.FirstOrDefault()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _appSettings.Site,
                audience: _appSettings.Audience,
                claims: claims,
                expires: Expires,
                signingCredentials: creds);

            return new LoginResponse()
            {
                Token = tokenHandler.WriteToken(token),
                Expiration = Expires,
                Check = true,
                Status = "success",
                Message = "successfully login"
            };
        }

    }
}