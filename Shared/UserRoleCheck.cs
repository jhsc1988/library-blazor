using Microsoft.AspNetCore.Components.Authorization;

namespace lib_blazor.Shared
{
    public class UserRoleCheck
    {
        private AuthenticationStateProvider AuthenticationStateProvider { get; }

        public UserRoleCheck(AuthenticationStateProvider authenticationStateProvider)
        {
            AuthenticationStateProvider = authenticationStateProvider;
        }

        public async Task<bool> IsUserAdminAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            return user.IsInRole("Admin");
        }
    }
}