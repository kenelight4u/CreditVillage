using AutoMapper;
using CreditVillageBackend.ViewModels.MappingViewModels;
using CreditVillageBackend.ViewModels.ResponseViewModels;

namespace CreditVillageBackend
{
    public class AppMapping : Profile
    {
        public AppMapping()
        {
            CreateMap<RegisterResponse, RegisterMapping>();

            CreateMap<ResendCodeResponse, ResendCodeMapping>();

            CreateMap<LoginResponse, LoginMapping>()
                .ForMember(dest => dest.Data, opt =>
                    opt.MapFrom(x => new LoginConfirm { Token = x.Token,
                                                            Expiration = x.Expiration
                                                        }));

            CreateMap<ForgetPasswordResponse, ForgetPasswordMapping>();

            CreateMap<GetUserResponse, GetUserMapping>();
              
            CreateMap<VerifyResponse, VerifyMapping>();

            CreateMap<ChangePasswordResponse, ChangePasswordMapping>();

            CreateMap<UpdateResponse, UpdateMapping>();

            CreateMap<EditResponse, EditMapping>();

            CreateMap<ResetPasswordResponse, ResetPasswordMapping>();
        }
    }
}