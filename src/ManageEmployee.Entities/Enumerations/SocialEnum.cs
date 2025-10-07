using System.ComponentModel;

namespace ManageEmployee.Entities.Enumerations;

public enum SocialEnum
{
    [Description("Facebook")]
    Facebook = 1,

    [Description("Instagram")]
    Instagram = 2,

    [Description("Twitter")]
    Twitter = 3,

    [Description("Youtube")]
    Youtube = 4,

    [Description("Whatsapp")]
    Whatsapp = 5,

    [Description("Printerest")]
    Printerest = 6,
}