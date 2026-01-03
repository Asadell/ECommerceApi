using ECommerceApi.Models;

namespace ECommerceApi.Helpers;

public interface IJwtHelper
{
    string GenerateToken(User user);
}