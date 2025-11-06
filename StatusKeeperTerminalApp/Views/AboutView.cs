using Terminal.Gui;

namespace StatusKeeperTerminalApp.Views;

public class AboutView : Window
{
    public AboutView()
    {
        Title = "Über Status Keeper";
        X = Pos.Center();
        Y = Pos.Center();
        Width = 60;
        Height = 18;

        InitializeComponents();

        // ESC schließt die View
        KeyDown += (e) =>
        {
            if (e.KeyEvent.Key == Key.Esc)
            {
                Application.RequestStop();
                e.Handled = true;
            }
        };
    }

    private void InitializeComponents()
    {
        var titleLabel = new Label("Status Keeper")
        {
            X = Pos.Center(),
            Y = 1,
            TextAlignment = TextAlignment.Centered
        };
        Add(titleLabel);

        var versionLabel = new Label("Version 2.0")
        {
            X = Pos.Center(),
            Y = 2,
            TextAlignment = TextAlignment.Centered
        };
        Add(versionLabel);

        var separator = new Label("─────────────────────────────────────────────────────")
        {
            X = Pos.Center(),
            Y = 3,
            TextAlignment = TextAlignment.Centered
        };
        Add(separator);

        var descriptionLabel = new Label("Automatische Mausbewegung zur Statuserhaltung")
        {
            X = Pos.Center(),
            Y = 5,
            TextAlignment = TextAlignment.Centered
        };
        Add(descriptionLabel);

        var featuresTitle = new Label("Funktionen:")
        {
            X = 2,
            Y = 7
        };
        Add(featuresTitle);

        var features = new string[]
        {
            "• Konfigurierbare Mausbewegungsprofile",
            "• Zufällige Intervalle und Bewegungen",
            "• Pausen- und Mittagspausenverwaltung",
            "• Arbeitszeiten mit Varianz",
            "• Mehrere Profile verwaltbar"
        };

        for (int i = 0; i < features.Length; i++)
        {
            var featureLabel = new Label(features[i])
            {
                X = 4,
                Y = 8 + i
            };
            Add(featureLabel);
        }

        var copyrightLabel = new Label($"© {DateTime.Now.Year} Status Keeper 2 (Marcel Braun)")
        {
            X = Pos.Center(),
            Y = 14,
            TextAlignment = TextAlignment.Centered
        };
        Add(copyrightLabel);

        var closeButton = new Button("Schließen")
        {
            X = Pos.Center(),
            Y = Pos.Bottom(this) - 2
        };
        closeButton.Clicked += () => Application.RequestStop();
        Add(closeButton);
    }
}
