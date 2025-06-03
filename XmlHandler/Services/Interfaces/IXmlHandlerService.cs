using XmlHandler.Model;

namespace XmlHandler.Services.Interfaces;

public interface IXmlHandlerService
{
    Task<bool> EnsureConfigFileExists();

    Task<UserInformation?> LoadUserInformation();

    Task<UserInformation?> LoadUserInformation(string path);

    Task<bool> SaveUserInformation(UserInformation userInfo);
}
