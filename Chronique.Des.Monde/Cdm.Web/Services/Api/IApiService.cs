using Cdm.Web.Models;

namespace Cdm.Web.Services.Api;

public interface IApiService
{
    Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request);
    Task<ApiResponse<RegisterResponse>> RegisterAsync(RegisterRequest request);
}