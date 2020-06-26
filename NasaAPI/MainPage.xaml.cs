using NasaAPI.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;


// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0xc0a

namespace NasaAPI
{
    /// <summary>
    /// Página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static MainPage Current;

        public ObservableCollection<MyPage> pages = new ObservableCollection<MyPage>
        {
            new MyPage() { Tag="Home", Tooltip="Imagen del día", Glyph=Symbol.Home, ClassType=typeof(Home) },
            new MyPage() { Tag="AcercaDe", Tooltip="Acerca de", Glyph=Symbol.ContactInfo, ClassType=typeof(AcercaDe) },
            //new MyPage() { Tag="Playlists", Tooltip="Listas de reproduccion", Glyph=Symbol.MusicInfo, ClassType=typeof(Playlists) }
            //new Scenario() { Name="Settings", Tooltip="Configuracion", Glyph=Symbol.Home, ClassType=typeof(Settings) }
        };

        public MainPage()
        {
            this.InitializeComponent();

            Current = this;

            CustomTitleBar();
        }

        private void CustomTitleBar()
        {
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;

            var titleBar = ApplicationView.GetForCurrentView().TitleBar;

            titleBar.ButtonForegroundColor = Colors.White;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonHoverForegroundColor = Colors.White;
            titleBar.ButtonHoverBackgroundColor = Color.FromArgb(30, 255, 255, 255);
            titleBar.ButtonPressedForegroundColor = Colors.White;
            titleBar.ButtonPressedBackgroundColor = Colors.DodgerBlue;

            titleBar.ButtonInactiveForegroundColor = Colors.Gray;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
        }

        private void navigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                NavViewNavigate("settings", args.RecommendedNavigationTransitionInfo);
            }
            else if (args.SelectedItemContainer != null)
            {
                var navItemName = args.SelectedItemContainer.Tag.ToString();

                NavViewNavigate(navItemName, args.RecommendedNavigationTransitionInfo);
            }
        }

        private void NavViewNavigate(string navItemName, NavigationTransitionInfo transitionInfo)
        {
            Type _page = null;

            if (navItemName == "settings")
            {
                _page = typeof(Settings);
            }
            else
            {
                var item = pages.FirstOrDefault(p => p.Tag.Equals(navItemName));
                _page = item.ClassType;
            }

            var preNavPageType = contentFrame.CurrentSourcePageType;

            if (!(_page is null) && !Type.Equals(preNavPageType, _page))
            {
                contentFrame.Navigate(_page, null, transitionInfo);
            }
        }

        private void navigationView_Loaded(object sender, RoutedEventArgs e)
        {
            contentFrame.Navigated += ContentFrame_Navigated;

            navigationView.SelectedItem = navigationView.MenuItems[0];

            NavViewNavigate("Home", new EntranceNavigationTransitionInfo());
        }

        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            navigationView.IsBackEnabled = contentFrame.CanGoBack;

            if (contentFrame.SourcePageType == typeof(Settings))
            {
                // SettingsItem is not part of NavView.MenuItems, and doesn't have a Tag.
                navigationView.SelectedItem = (NavigationViewItem)navigationView.SettingsItem;
                navigationView.Header = "Configuración";
            }
            else if (contentFrame.SourcePageType != null)
            {
                var item = pages.FirstOrDefault(p => p.ClassType == e.SourcePageType);

                navigationView.SelectedItem = navigationView.MenuItems
                    .OfType<NavigationViewItem>()
                    .First(n => n.Tag.Equals(item.Tag));

                navigationView.Header = ((NavigationViewItem)navigationView.SelectedItem)?.Content?.ToString();
            }
        }

        private void navigationView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            if (contentFrame.CanGoBack)
            {
                contentFrame.GoBack();
            }
        }
    }

    public class MyPage
    {
        public string Tag { get; set; }

        public string Tooltip { get; set; }

        public Symbol Glyph { get; set; }

        public Type ClassType { get; set; }
    }
}
