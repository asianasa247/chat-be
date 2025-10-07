using ManageEmployee.DataTransferObject.SocialModels;

namespace ManageEmployee.Services.Interfaces.Socials
{

    public interface ISocials
    {
        Task<SocialVerifiedModel> GetUserInfoGoogle(string accessToken);
        Task<SocialVerifiedModel> GetUserInfoFacebook(string accessToken);
    }
};
