using Microsoft.AspNetCore.Components;
using Cdm.Web.Services.Authentication;

namespace Cdm.Web.Components.Layout;

public partial class MainLayout : LayoutComponentBase
{
    [Inject] private IAuthenticationService AuthService { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
}