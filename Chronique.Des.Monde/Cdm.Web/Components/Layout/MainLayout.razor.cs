using Microsoft.AspNetCore.Components;
using Cdm.Web.Services.Authentication;

namespace Cdm.Web.Components.Layout;

public partial class MainLayout : LayoutComponentBase
{
    // 🔧 TEMPORARY: Comment out auth service for demo mode
    // [Inject] private IAuthenticationService AuthService { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
}