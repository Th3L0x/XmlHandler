using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Xml.Serialization;
using XmlHandler.Util.Enums;

namespace XmlHandler.Model;

[XmlRoot("UserInformation")]
public class UserInformation
{
    [XmlArray("Users")]
    [XmlArrayItem("User")]
    public List<User> Users { get; set; } = [];
}

public partial class User : ObservableObject, IDataErrorInfo
{
    private string? _firstName;

    [XmlAttribute]
    public string? FirstName
    {
        get => _firstName;
        set => SetProperty(ref _firstName, value);
    }

    private string? _lastName;
    [XmlAttribute]
    public string? LastName
    {
        get => _lastName;
        set => SetProperty(ref _lastName, value);
    }

    private CivilState _civilStateConverted;
    [XmlIgnore]
    public CivilState CivilStateConverted
    {
        get => (CivilState)CivilState;
        set
        {
            SetProperty(ref _civilStateConverted, value);
            CivilState = (int)value;
        }
    }

    [XmlAttribute]
    public int CivilState { get; set; }

    private bool _isSelected = false;
    [XmlIgnore]
    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }

    public string this[string columnName]
    {
        get
        {
            if(columnName == nameof(FirstName) && FirstName == null)
                return $"{nameof(FirstName)} cannot be empty";
            if (columnName == nameof(FirstName) && FirstName != null && FirstName.Any(char.IsDigit))
                return $"{nameof(FirstName)}  cannot contain numbers";
            if (columnName == nameof(LastName) && LastName == null)
                return $"{nameof(LastName)} cannot be empty";
            if (columnName == nameof(LastName) && LastName != null && LastName.Any(char.IsDigit))
                return $"{nameof(LastName)}  cannot contain numbers";
            return string.Empty;
        }
    }

    public string Error => string.Empty;
}
