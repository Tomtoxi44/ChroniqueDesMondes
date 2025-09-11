using MudBlazor;

namespace Cdm.Web.Themes;

/// <summary>
/// Thème D&D personnalisé pour MudBlazor avec couleurs fantasy
/// </summary>
public static class DndTheme
{
    /// <summary>
    /// Thème principal D&D avec palette de couleurs fantasy
    /// </summary>
    public static MudTheme Theme => new()
    {
        Palette = new PaletteLight()
        {
            Primary = "#8B0000",        // Rouge sang - Actions offensives
            Secondary = "#DAA520",      // Or ancien - Actions importantes
            Tertiary = "#228B22",       // Vert forêt - Actions positives
            Success = "#228B22",        // Vert forêt
            Warning = "#FF6B35",        // Orange flamme
            Error = "#DC143C",          // Rouge vif
            Info = "#6A5ACD",           // Violet mystique
            Dark = "#2D1B0B",           // Brun profond
            Surface = "#F4E4BC",        // Parchemin
            Background = "#FDF8F1",     // Crème
            AppbarBackground = "#2D1B0B", // Brun profond
            AppbarText = "#F4E4BC",     // Parchemin
            DrawerBackground = "#F4E4BC", // Parchemin
            TextPrimary = "#2D1B0B",    // Brun profond
            TextSecondary = "#5D4E37",  // Brun moyen
        },
        
        PaletteDark = new PaletteDark()
        {
            Primary = "#CD5C5C",        // Rouge indien - Plus doux pour dark
            Secondary = "#FFD700",      // Or - Plus lumineux
            Tertiary = "#32CD32",       // Vert lime - Plus visible
            Success = "#32CD32",        // Vert lime
            Warning = "#FF7F50",        // Corail
            Error = "#FF6347",          // Tomate rouge
            Info = "#9370DB",           // Violet medium
            Dark = "#0F0A05",           // Noir brun
            Surface = "#2D1B0B",        // Brun profond
            Background = "#1A0F08",     // Très sombre
            AppbarBackground = "#0F0A05", // Noir brun
            AppbarText = "#F4E4BC",     // Parchemin
            DrawerBackground = "#2D1B0B", // Brun profond
            TextPrimary = "#F4E4BC",    // Parchemin
            TextSecondary = "#D2B48C",  // Tan
        },

        Typography = new Typography()
        {
            Default = new Default()
            {
                FontFamily = new[] { "Inter", "Roboto", "Segoe UI", "sans-serif" },
                FontSize = "0.875rem",
                FontWeight = 400,
                LineHeight = 1.43,
                LetterSpacing = "0.01071em"
            },
            H1 = new H1()
            {
                FontFamily = new[] { "Cinzel", "serif" },
                FontSize = "2.5rem",
                FontWeight = 700,
                LineHeight = 1.2,
                LetterSpacing = "-0.01562em"
            },
            H2 = new H2()
            {
                FontFamily = new[] { "Cinzel", "serif" },
                FontSize = "2rem",
                FontWeight = 600,
                LineHeight = 1.3,
                LetterSpacing = "-0.00833em"
            },
            H3 = new H3()
            {
                FontFamily = new[] { "Cinzel", "serif" },
                FontSize = "1.75rem",
                FontWeight = 600,
                LineHeight = 1.4,
                LetterSpacing = "0em"
            },
            H4 = new H4()
            {
                FontFamily = new[] { "Inter", "sans-serif" },
                FontSize = "1.5rem",
                FontWeight = 600,
                LineHeight = 1.4,
                LetterSpacing = "0.00735em"
            },
            H5 = new H5()
            {
                FontFamily = new[] { "Inter", "sans-serif" },
                FontSize = "1.25rem",
                FontWeight = 600,
                LineHeight = 1.5,
                LetterSpacing = "0em"
            },
            H6 = new H6()
            {
                FontFamily = new[] { "Inter", "sans-serif" },
                FontSize = "1.125rem",
                FontWeight = 600,
                LineHeight = 1.6,
                LetterSpacing = "0.0075em"
            },
            Button = new Button()
            {
                FontFamily = new[] { "Inter", "sans-serif" },
                FontSize = "0.875rem",
                FontWeight = 500,
                LineHeight = 1.75,
                LetterSpacing = "0.02857em",
                TextTransform = "uppercase"
            },
            Body1 = new Body1()
            {
                FontFamily = new[] { "Inter", "sans-serif" },
                FontSize = "1rem",
                FontWeight = 400,
                LineHeight = 1.5,
                LetterSpacing = "0.00938em"
            },
            Body2 = new Body2()
            {
                FontFamily = new[] { "Inter", "sans-serif" },
                FontSize = "0.875rem",
                FontWeight = 400,
                LineHeight = 1.43,
                LetterSpacing = "0.01071em"
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