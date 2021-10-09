using CreditVillageBackend.ViewModels.RequestViewModels;
using CreditVillageBackend.ViewModels.ResponseViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CreditVillageBackend
{
    public interface IAccount
    {
        Task<RegisterResponse> UserRegisterAsync(RegisterRequest userRequest);

        Task<RegisterResponse> AdminRegisterAsync(RegisterRequest userRequest);

        Task<ResendCodeResponse> UserResendCodeAsync(ResendCodeRequest userRequest);

        Task<VerifyResponse> UserVerifyAsync(VerifyRequest userRequest);

        Task<LoginResponse> LoginAsync(LoginRequest userRequest);

        Task<ForgetPasswordResponse> UserForgetPasswordAsync(ForgetPasswordRequest userRequest);

        Task<ChangePasswordResponse> ChangePasswordAsync(string GetUserId, ChangePasswordRequest changePasswordRequest);

        Task<GetUserResponse> GetUserAsync(string UserId);

        Task<UpdateResponse> UpdateUserAsync(string GetUserId, UpdateRequest updateRequest);

        Task<EditResponse> EditUserDetailsAsync(string GetUserId, EditRequest editRequest);

        Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordRequest userRequest);
    }
}