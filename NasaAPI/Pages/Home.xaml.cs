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
using System.Web;
using Microsoft.Toolkit.Uwp.Helpers;
using System.Buffers.Text;
using Windows.Storage.Provider;
using NasaAPI.services;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace NasaAPI.Pages
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class Home : Page
    {
        string dataURL = "https://api.nasa.gov/planetary/apod?api_key=XdRrmURyk5bW91jnAyoHbaAngJrF8vKIiQiZI6AV";

        string imagesSubdirectory = "NasaAPI";

        NasaJSON nasa;

        StorageFile fileFondo;

        bool internet = false;

        DispatcherTimer timer = new DispatcherTimer();


        public Home()
        {
            this.InitializeComponent();

            // Por defecto intento traer los datos
            LoadData();

            // Si no se pudo, inicia un timer que intenta conexion cada 10 segundos
            if (!internet)
            {
                timer.Tick += Timer_Tick;
                timer.Interval = new TimeSpan(0, 0, 10);

                timer.Start();
            }
        }

        private void Timer_Tick(object sender, object e)
        {
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                var json = new WebClient().DownloadString(dataURL);

                internet = true;

                // Si pudo traer los datos cancelo el timer
                if (timer.IsEnabled)
                {
                    timer.Stop();
                }

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

                // Desactivo el 1er proggres
                progressImage.IsActive = false;

                // btn Aplicar wallpaper activado
                btnAplicar.IsEnabled = true;

                // btn Guardar como activado
                btnGuardarComo.IsEnabled = true;

                // Si opcion actualizaciones automaticas, aplico la imagen del dia
                if (SettingsService.Instance.ActualizacionAutomatica && nasa.title != "Default Image")
                {
                    btnAplicar_Click(null, null);
                }
            }
            catch (Exception)
            {
                // Sino muestro error
                textBlockWallpaper.Visibility = Visibility.Visible;
                textBlockWallpaper.Text = "Sin conexion";
            }  
        }

        private async Task<bool> DownloadImage(string url, string fileName)
        {
            try
            {
                // Intento descargar la imagen
                StorageFolder pictures = KnownFolders.PicturesLibrary;

                var rootFolder = await pictures.CreateFolderAsync(imagesSubdirectory, CreationCollisionOption.OpenIfExists);

                fileFondo = await rootFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

                using (HttpClient client = new HttpClient())
                {
                    byte[] buffer = await client.GetByteArrayAsync(url);

                    using (Stream stream = await fileFondo.OpenStreamForWriteAsync())
                    {
                        stream.Write(buffer, 0, buffer.Length);
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async void SetWallpaper()
        {
            if (UserProfilePersonalizationSettings.IsSupported())
            {
                progressImage2.IsActive = true;
                textBlockWallpaper.Visibility = Visibility.Visible;
                textBlockWallpaper.Text = "Descargando wallpaper";

                btnAplicar.IsEnabled = false;

                bool descarga = await DownloadImage(nasa.hdurl, $"{nasa.title}.jpg");

                if (descarga)
                {
                    textBlockWallpaper.Text = "Aplicando wallpaper";

                    StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                    StorageFile temp = await fileFondo.CopyAsync(storageFolder, $"{nasa.title}.jpg", NameCollisionOption.ReplaceExisting);

                    UserProfilePersonalizationSettings profileSettings = UserProfilePersonalizationSettings.Current;
                    bool res = await profileSettings.TrySetWallpaperImageAsync(temp);

                    if (res)
                    {
                        progressImage2.IsActive = false;
                        textBlockWallpaper.Visibility = Visibility.Collapsed;
                        btnAplicar.IsEnabled = true;
                    }
                    else
                    {
                        progressImage2.IsActive = false;
                        textBlockWallpaper.Text = "Error al establecer wallpaper";
                        btnAplicar.IsEnabled = true;
                    }
                }
                else
                {
                    progressImage2.IsActive = false;
                    textBlockWallpaper.Text = "Error en la descarga";
                    btnAplicar.IsEnabled = true;
                }
            }
        }

        private void ShowState(bool progressIsActive, bool textWallpaperVisibility, string textWallpaper, bool btnAplicarIsEnabled)
        {
            progressImage2.IsActive = progressIsActive;
            textBlockWallpaper.Text = textWallpaper;
            btnAplicar.IsEnabled = btnAplicarIsEnabled;

            if (textWallpaperVisibility)
            {
                textBlockWallpaper.Visibility = Visibility.Visible;
            }
        }

        private void btnAplicar_Click(object sender, RoutedEventArgs e)
        {
            SetWallpaper();
        }

        private async void btnGuardarComo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var savePicker = new Windows.Storage.Pickers.FileSavePicker();

                savePicker.FileTypeChoices.Add("JPEG-Image", new List<string>() { ".jpg" });

                savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
                savePicker.SuggestedSaveFile = fileFondo;
                savePicker.SuggestedFileName = nasa.title;

                StorageFile file = await savePicker.PickSaveFileAsync();

                if (file != null)
                {
                    progressImage2.IsActive = true;
                    textBlockWallpaper.Visibility = Visibility.Visible;
                    textBlockWallpaper.Text = "Descargando wallpaper";

                    if (fileFondo != null)
                    {
                        await fileFondo.CopyAndReplaceAsync(file);
                    }
                    else
                    {
                        bool descarga = await DownloadImage(nasa.hdurl, nasa.title);

                        if (descarga)
                        {
                            await fileFondo.CopyAndReplaceAsync(file);
                        }
                        else
                        {
                            progressImage2.IsActive = false;
                            textBlockWallpaper.Text = "Error en la descarga";
                            btnAplicar.IsEnabled = true;
                        }
                    }

                    progressImage2.IsActive = false;
                    textBlockWallpaper.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception)
            {
                textBlockWallpaper.Visibility = Visibility.Visible;
                textBlockWallpaper.Text = "Error al realizar la operacion";
            }
            
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
