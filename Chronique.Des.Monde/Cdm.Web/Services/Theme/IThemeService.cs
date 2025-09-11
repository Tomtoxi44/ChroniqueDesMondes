using MudBlazor;

namespace Cdm.Web.Services.Theme;

/// <summary>
/// Service de gestion du thème D&D pour MudBlazor
/// </summary>
public interface IThemeService
{
    /// <summary>
    /// Thème actuel
    /// </summary>
    MudTheme CurrentTheme { get; }
    
    /// <summary>
    /// Mode sombre activé
    /// </summary>
    bool IsDarkMode { get; }
    
    /// <summary>
    /// Bascule entre mode clair et sombre
    /// </summary>
    Task ToggleDarkModeAsync();
    
    /// <summary>
    /// Définit le mode sombre
    /// </summary>
    Task SetDarkModeAsync(bool isDark);
    
    /// <summary>
    /// Événement déclenché lors du changement de thème
    /// </summary>
    event EventHandler? ThemeChanged;
}

/// <summary>
/// Implémentation du service de thème D&D
/// </summary>
public class ThemeService : IThemeService
{
    private bool _isDarkMode = true; // Mode sombre par défaut pour D&D
    
    public MudTheme CurrentTheme => Themes.DndTheme.Theme;
    
    public bool IsDarkMode => _isDarkMode;
    
    public event EventHandler? ThemeChanged;

    public Task ToggleDarkModeAsync()
    {
        return SetDarkModeAsync(!_isDarkMode);
    }

    public Task SetDarkModeAsync(bool isDark)
    {
        if (_isDarkMode != isDark)
        {
            _isDarkMode = isDark;
            ThemeChanged?.Invoke(this, EventArgs.Empty);
        }
        
        return Task.CompletedTask;
    }
}