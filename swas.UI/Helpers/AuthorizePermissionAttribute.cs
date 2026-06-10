using Microsoft.AspNetCore.Authorization;

namespace swas.UI.Helpers
{
    public class AuthorizePermissionAttribute : AuthorizeAttribute
    {
        public AuthorizePermissionAttribute(string permission)
        {
            Policy = $"Permission:{permission}";
        }
    }
}
