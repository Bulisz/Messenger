using System.Text.Json.Serialization;

namespace Messenger.Backend.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Role
{
    Admin,
    User
}
