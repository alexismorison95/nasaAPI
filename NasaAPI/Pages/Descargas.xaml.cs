using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
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

        public Descargas()
        {
            this.InitializeComponent();

            Cargar();
        }

        private async void Cargar()
        {
            StorageFolder pictures = KnownFolders.PicturesLibrary;

            var rootFolder = await pictures.CreateFolderAsync("NasaAPI", CreationCollisionOption.OpenIfExists);

            IReadOnlyList<StorageFile> files = await rootFolder.GetFilesAsync();

            foreach (var item in files)
            {
                ImageFile img = new ImageFile();

                using (IRandomAccessStream randomAccessStream = await item.OpenAsync(FileAccessMode.Read))
                {
                    img.IName = new BitmapImage();

                    await img.IName.SetSourceAsync(randomAccessStream);

                    items.Add(img);
                }  
            }
            AdaptiveGridViewControl.ItemsSource = items;
        }
    }

    public class ImageFile
    {
        public BitmapImage IName
        {
            get;
            set;
        }
    }
}
