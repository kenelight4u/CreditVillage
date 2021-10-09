using System.Threading.Tasks;
using AutoMapper;
using CreditVillageBackend.Helpers;
using CreditVillageBackend.ViewModels.MappingViewModels;
using CreditVillageBackend.ViewModels.RequestViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CreditVillageBackend.Controllers
{
    [ApiController]
    public class AccountController : ControllerBase
    {      
        private readonly IAccount _account;

        private readonly IMailService _mailService;

        private readonly IMapper _mapper;

        public AccountController(IAccount account,
            IMailService mailService,
            IMapper mapper)
        {
            _account = account;
            _mailService = mailService;
            this._mapper = mapper;
        }


        [HttpPost("v1/register")]
        [AllowAnonymous]
        public async Task<IActionResult> UserRegister([FromBody] RegisterRequest userRequest)
        {
            var authResponse = await _account.UserRegisterAsync(userRequest);

            if (!authResponse.Check)
            {
                return Ok(_mapper.Map<RegisterMapping>(authResponse));
            }
         
            return Ok( _mapper.Map<RegisterMapping>(authResponse));
        }


        [HttpPost("v1/admin/register")]
        [AllowAnonymous]
        public async Task<IActionResult> AdminRegister([FromBody] RegisterRequest userRequest)
        {
            var authResponse = await _account.AdminRegisterAsync(userRequest);

            if (!authResponse.Check)
            {
                return Ok(_mapper.Map<RegisterMapping>(authResponse));
            }

            return Ok( _mapper.Map<RegisterMapping>(authResponse));
        }


        [HttpPost("v1/login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest userRequest)
        {
            var authUserResponse = await _account.LoginAsync(userRequest);

            if (!authUserResponse.Check)
            {
                return Ok(_mapper.Map<LoginMapping>(authUserResponse));
            }

            return Ok(_mapper.Map<LoginMapping>(authUserResponse));
        }


        [HttpPost("v1/forgetpassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordRequest userRequest)
        {
            var userResponse = await _account.UserForgetPasswordAsync(userRequest);

            if (!userResponse.Check)
            {
                return Ok(_mapper.Map<ForgetPasswordMapping>(userResponse));
            }

            //Front end guy will give me the URL that I needed
            var callbackUrl = $"/Account/ResetPassword?UserId={userResponse.Email}&Code={userResponse.Code}";
           // var callbackUrl = Url.Action("ForgetPassword", "Manage", new { Email = userResponse.Email, Code = userResponse.Code }, HttpContext.Request.Scheme);
          
            await _mailService.SendEmailAsync(
                    userResponse.Email,
                    "CREDIT VILLAGE - Reset Password",
                    $"Please Reset your password by clicking this link: <a href=\"{callbackUrl}\">Click here </a>"
                    );

            return Ok( _mapper.Map<ForgetPasswordMapping>(userResponse));
        }

        [HttpPost("v1/resetpassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest resetPasswordRequest)
        {
            var passwordResponse = await _account.ResetPasswordAsync(resetPasswordRequest);

            if (!passwordResponse.Check)
            {
                return Ok(_mapper.Map<ResetPasswordMapping>(passwordResponse));
            }

            return Ok(_mapper.Map<ResetPasswordMapping>(passwordResponse));
        }


        [HttpGet("v1/user")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetUser()
        {
            var user = await _account.GetUserAsync(GetUserId());

            return Ok(_mapper.Map<GetUserMapping>(user));
        }


        [HttpPut("v1/updateUser")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateRequest updateRequest)
        {
            var userUpdate = await _account.UpdateUserAsync(GetUserId(), updateRequest);

            if (!userUpdate.Check)
            {
                return Ok(_mapper.Map<UpdateMapping>(userUpdate));
            }

            return Ok(_mapper.Map<UpdateMapping>(userUpdate));
        }

        [HttpPut("v1/editUserDetails")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> EditUserDetails([FromForm] EditRequest editRequest)
        {
            var userUpdate = await _account.EditUserDetailsAsync(GetUserId(), editRequest);

            if (!userUpdate.Check)
            {
                return Ok(_mapper.Map<EditMapping>(userUpdate));
            }

            return Ok(_mapper.Map<EditMapping>(userUpdate));
        }


        [HttpPut("v1/user/resendCode")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendCode([FromBody] ResendCodeRequest userRequest)
        {
            var authResponse = await _account.UserResendCodeAsync(userRequest);

            if (!authResponse.Check)
            {
                return Ok(_mapper.Map<ResendCodeMapping>(authResponse));
            }

            return Ok( _mapper.Map<ResendCodeMapping>(authResponse));
        }

        
        [HttpPut("v1/user/verify")]
        [AllowAnonymous]
        public async Task<IActionResult> UserVerify([FromBody] VerifyRequest userRequest)
        {
            var verifyResponse = await _account.UserVerifyAsync(userRequest);

            if (!verifyResponse.Check)
            {
                return Ok(_mapper.Map<VerifyMapping>(verifyResponse));
            }

            return Ok( _mapper.Map<VerifyMapping>(verifyResponse));
        }


        [HttpPut("v1/user/changepassword")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest changePasswordRequest)
        {
            var passwordResponse = await _account.ChangePasswordAsync(GetUserId(), changePasswordRequest);

            if (!passwordResponse.Check)
            {
                return Ok(_mapper.Map<ChangePasswordMapping>(passwordResponse));
            }

            return Ok(_mapper.Map<ChangePasswordMapping>(passwordResponse));
        }


        private string GetUserId()
        {
            return HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
        }       
    }
}