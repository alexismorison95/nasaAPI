using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json;
using Windows.UI.Xaml.Media.Imaging;
using Windows.System.UserProfile;
using Windows.Storage;
using System.Threading.Tasks;
using System.Net.Http;
using Windows.UI;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace NasaAPI.Pages
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class Home : Page
    {
        string dataURL = "https://api.nasa.gov/planetary/apod?api_key=XdRrmURyk5bW91jnAyoHbaAngJrF8vKIiQiZI6AV";

        string fondoRuta = "ms-appdata:///local/DownloadedImages/fondo";

        NasaJSON nasa;

        StorageFile fileFondo;
        bool isDescargandoFondo = true;

        public Home()
        {
            this.InitializeComponent();

            LoadData();
        }

        private async void LoadData()
        {
            var json = new WebClient().DownloadString(dataURL);

            nasa = JsonConvert.DeserializeObject<NasaJSON>(json);

            textBlockDescripcion.Text = nasa.explanation;

            textBlockTitulo.Text = nasa.title;

            if (nasa.copyright != null)
            {
                textBlockAutor.Text = nasa.copyright;
            }
            else
            {
                textBlockAutor.Text = "";
            }

            // Descargo la vista preliminar

            Uri uri = new Uri(nasa.url);
            ImageSource imgSource = new BitmapImage(uri);
            imageVista.Source = imgSource;

            progressImage.IsActive = false;

            // Descargo el wallpaper

            if (isDescargandoFondo)
            {
                progressImage2.IsActive = true;
                textBlockWallpaper.Visibility = Visibility.Visible;

                string imagenString = await DownloadImage(nasa.hdurl, "fondo");
                Uri uriFondo = new Uri(imagenString);

                fileFondo = await StorageFile.GetFileFromApplicationUriAsync(uriFondo);

                isDescargandoFondo = false;
                progressImage2.IsActive = false;
                textBlockWallpaper.Visibility = Visibility.Collapsed;

                btnAplicar.IsEnabled = true;
            } 
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (UserProfilePersonalizationSettings.IsSupported())
            {
                //LoadingControl.IsLoading = true;
                //progressRing.IsActive = true;

                UserProfilePersonalizationSettings profileSettings = UserProfilePersonalizationSettings.Current;

                await profileSettings.TrySetWallpaperImageAsync(fileFondo);

                //LoadingControl.IsLoading = false;
                //progressRing.IsActive = false;
            }
        }

        private async Task<String> DownloadImage(string url, String fileName)
        {
            const String imagesSubdirectory = "DownloadedImages";

            var rootFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(imagesSubdirectory, CreationCollisionOption.OpenIfExists);

            var storageFile = await rootFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

            using (HttpClient client = new HttpClient())
            {
                byte[] buffer = await client.GetByteArrayAsync(url);
                using (Stream stream = await storageFile.OpenStreamForWriteAsync())
                    stream.Write(buffer, 0, buffer.Length);
            }

            // Use this path to load image
            String newPath = String.Format("ms-appdata:///local/{0}/{1}", imagesSubdirectory, fileName);

            return newPath;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }

    public class NasaJSON
    {
        public string copyright { get; set; }
        public string date { get; set; }
        public string explanation { get; set; }
        public string hdurl { get; set; }
        public string media_type { get; set; }
        public string service_version { get; set; }
        public string title { get; set; }
        public string url { get; set; }
    }
}
