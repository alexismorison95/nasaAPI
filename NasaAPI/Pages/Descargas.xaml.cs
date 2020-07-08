using Microsoft.Toolkit.Uwp.UI.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System.UserProfile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace NasaAPI.Pages
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class Descargas : Page
    {
        ObservableCollection<ImageFile> items = new ObservableCollection<ImageFile>();

        IReadOnlyList<StorageFile> files;

        ImageFile imagenSeleccionada;

        public Descargas()
        {
            this.InitializeComponent();

            LoadImagenesDescargadas();
        }

        private async void LoadImagenesDescargadas()
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;

            files = await storageFolder.GetFilesAsync();

            if (files.Count > 0)
            {
                ObservableCollection<ImageFile> itemsTemp = new ObservableCollection<ImageFile>();

                for (int i = 0; i < files.Count; i++)
                {
                    ImageFile img = new ImageFile();

                    using (IRandomAccessStream randomAccessStream = await files[i].OpenAsync(FileAccessMode.Read))
                    {
                        img.Image = new BitmapImage();

                        await img.Image.SetSourceAsync(randomAccessStream);

                        img.Nombre = files[i].DisplayName;

                        img.Posicion = i;

                        itemsTemp.Add(img);
                    }
                }

                items = itemsTemp;

                AdaptiveGridViewControl.ItemsSource = items;

                btnEliminarTodo.IsEnabled = true;
            }
            else
            {
                btnEliminarTodo.IsEnabled = false;
            }
        }

        private void AdaptiveGridViewControl_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem != null)
            {
                imagenSeleccionada = e.ClickedItem as ImageFile;

                opciones.Visibility = Visibility.Visible;

                textblockImgSeleccionada.Text = imagenSeleccionada.Nombre;
            }
        }

        private async void btnEliminarTodo_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog eliminarTodosDialog = new ContentDialog
            {
                Title = "Eliminar todos los archivos?",
                Content = "Una vez eliminados todos los wallpapers no sera posible recuperarlos. Desea continuar?",
                CloseButtonText = "Cancelar",
                PrimaryButtonText = "Eliminar todos"
            };

            ContentDialogResult result = await eliminarTodosDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                foreach (var item in files)
                {
                    await item.DeleteAsync();
                }

                imagenSeleccionada = null;

                items.Clear();

                files = null;

                opciones.Visibility = Visibility.Collapsed;

                btnEliminarTodo.IsEnabled = false;
            }
        }

        private async void btnAplicar_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog setWallpaperDialog = new ContentDialog
            {
                Title = "Aplicar wallpaper como fondo de pantalla?",
                Content = $"Desea aplicar la imagen {imagenSeleccionada.Nombre} como fondo de pantalla?",
                CloseButtonText = "Cancelar",
                PrimaryButtonText = "Aplicar"
            };

            ContentDialogResult result = await setWallpaperDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                UserProfilePersonalizationSettings profileSettings = UserProfilePersonalizationSettings.Current;

                await profileSettings.TrySetWallpaperImageAsync(files[imagenSeleccionada.Posicion]);
            }
        }

        private async void btnGuardarComo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var savePicker = new Windows.Storage.Pickers.FileSavePicker();

                savePicker.FileTypeChoices.Add("JPEG-Image", new List<string>() { ".jpg" });

                savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
                savePicker.SuggestedSaveFile = files[imagenSeleccionada.Posicion];
                savePicker.SuggestedFileName = imagenSeleccionada.Nombre;

                StorageFile file = await savePicker.PickSaveFileAsync();

                if (file != null)
                {
                    await files[imagenSeleccionada.Posicion].CopyAndReplaceAsync(file);
                }
            }
            catch (Exception)
            {
                //
            }
        }

        private async void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog eliminarTodosDialog = new ContentDialog
            {
                Title = "Eliminar el archivo?",
                Content = "Una vez eliminado el wallpaper no sera posible recuperarlo. Desea continuar?",
                CloseButtonText = "Cancelar",
                PrimaryButtonText = "Eliminar"
            };

            ContentDialogResult result = await eliminarTodosDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                await files[imagenSeleccionada.Posicion].DeleteAsync();

                items.RemoveAt(imagenSeleccionada.Posicion);

                imagenSeleccionada = null;

                LoadImagenesDescargadas();

                opciones.Visibility = Visibility.Collapsed;
            }
        }
    }

    public class ImageFile
    {
        public BitmapImage Image
        {
            get;
            set;
        }

        public string Nombre
        {
            get;
            set;
        }

        public int Posicion
        {
            get;
            set;
        }
    }
}
