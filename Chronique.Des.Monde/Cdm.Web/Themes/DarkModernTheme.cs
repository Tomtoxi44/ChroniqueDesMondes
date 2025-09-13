using MudBlazor;

namespace Cdm.Web.Themes;

/// <summary>
/// Thème sombre moderne et professionnel pour Chronique des Mondes
/// </summary>
public static class DarkModernTheme
{
    public static MudTheme Theme => new()
    {
        PaletteDark = new PaletteDark()
        {
            // Couleurs principales - Thème sombre moderne
            Primary = "#4A90E2",           // Bleu moderne
            Secondary = "#7B68EE",         // Violet élégant  
            Tertiary = "#50C878",          // Vert émeraude
            Success = "#50C878",           // Vert succès
            Warning = "#FFA500",           // Orange warning
            Error = "#FF4444",             // Rouge erreur
            Info = "#4A90E2",              // Bleu info
            
            // Arrière-plans sombres
            Background = "#0F0F0F",        // Noir profond
            Surface = "#1A1A1A",           // Gris très sombre
            AppbarBackground = "#111111",   // Noir légèrement moins profond
            DrawerBackground = "#1A1A1A",   // Même que surface
            
            // Textes
            TextPrimary = "#FFFFFF",        // Blanc pur
            TextSecondary = "#B0B0B0",      // Gris clair
            TextDisabled = "#666666",       // Gris moyen
            
            // Autres
            Divider = "#333333",            // Gris foncé pour les séparateurs
            ActionDefault = "#FFFFFF",      // Blanc pour les actions
        },

        Typography = new Typography()
        {
            Default = new Default()
            {
                FontFamily = new[] { "Inter", "Segoe UI", "system-ui", "sans-serif" },
                FontSize = "14px",
                FontWeight = 400,
                LineHeight = 1.5,
            },
            H1 = new H1()
            {
                FontFamily = new[] { "Inter", "sans-serif" },
                FontSize = "2.5rem",
                FontWeight = 300,
                LineHeight = 1.2,
            },
            H2 = new H2()
            {
                FontFamily = new[] { "Inter", "sans-serif" },
                FontSize = "2rem",
                FontWeight = 300,
                LineHeight = 1.3,
            },
            H3 = new H3()
            {
                FontFamily = new[] { "Inter", "sans-serif" },
                FontSize = "1.75rem",
                FontWeight = 400,
                LineHeight = 1.4,
            },
            H4 = new H4()
            {
                FontFamily = new[] { "Inter", "sans-serif" },
                FontSize = "1.5rem",
                FontWeight = 500,
                LineHeight = 1.4,
            },
            H5 = new H5()
            {
                FontFamily = new[] { "Inter", "sans-serif" },
                FontSize = "1.25rem",
                FontWeight = 500,
                LineHeight = 1.5,
            },
            H6 = new H6()
            {
                FontFamily = new[] { "Inter", "sans-serif" },
                FontSize = "1.125rem",
                FontWeight = 600,
                LineHeight = 1.6,
            },
            Button = new Button()
            {
                FontFamily = new[] { "Inter", "sans-serif" },
                FontSize = "14px",
                FontWeight = 500,
                LineHeight = 1.5,
            },
            Body1 = new Body1()
            {
                FontFamily = new[] { "Inter", "sans-serif" },
                FontSize = "16px",
                FontWeight = 400,
                LineHeight = 1.5,
            },
            Body2 = new Body2()
            {
                FontFamily = new[] { "Inter", "sans-serif" },
                FontSize = "14px",
                FontWeight = 400,
                LineHeight = 1.4,
            }
        },

        LayoutProperties = new LayoutProperties()
        {
            DefaultBorderRadius = "8px",
            AppbarHeight = "64px",
            DrawerWidthLeft = "280px",
            DrawerWidthRight = "280px"
        }
    };
}