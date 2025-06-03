using Microsoft.Extensions.Logging;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using XmlHandler.Model;
using XmlHandler.Services.Interfaces;

namespace XmlHandler.Services;

public class XmlHandlerService(ILogger<XmlHandlerService> logger) : IXmlHandlerService
{
    private static readonly string _fileName = "Employee.xml";
    private static string _targetPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        _fileName
    );

    private readonly ILogger<XmlHandlerService> _logger = logger;

    public async Task<UserInformation?> LoadUserInformation()
    {
        try
        {
            var serializer = new XmlSerializer(typeof(UserInformation));
            using var reader = new StreamReader(_targetPath);
            return await Task.FromResult((UserInformation?)serializer.Deserialize(reader));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return null;
        }
    }

    public async Task<UserInformation?> LoadUserInformation(string path)
    {
        try
        {
            _targetPath = path;
            var serializer = new XmlSerializer(typeof(UserInformation));
            using var reader = new StreamReader(path);
            return await Task.FromResult((UserInformation?)serializer.Deserialize(reader));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return null;
        }
    }



    public async Task<bool> SaveUserInformation(UserInformation userInfo)
    {
        try
        {
            var serializer = new XmlSerializer(typeof(UserInformation));
            using var writer = new StreamWriter(_targetPath, false, Encoding.UTF8);
            await Task.Run(() => serializer.Serialize(writer, userInfo));
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return false;
        }
    }

    public async Task<bool> EnsureConfigFileExists()
    {
        try
        {
            if (File.Exists(_targetPath))
                return true;

            Directory.CreateDirectory(Path.GetDirectoryName(_targetPath)!);

            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = $"XmlHandler.Resources.{_fileName}"; // Update this!

            using Stream? resourceStream = assembly.GetManifestResourceStream(resourceName);
            if (resourceStream == null)
                throw new FileNotFoundException("Embedded resource not found: " + resourceName);

            using FileStream fileStream = new FileStream(_targetPath, FileMode.Create, FileAccess.Write);
            await resourceStream.CopyToAsync(fileStream).ConfigureAwait(false);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return false;
        }
    }
}
