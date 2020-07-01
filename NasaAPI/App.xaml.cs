using NasaAPI.services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace NasaAPI
{
    /// <summary>
    /// Proporciona un comportamiento específico de la aplicación para complementar la clase Application predeterminada.
    /// </summary>
    sealed partial class App : Application
    {
        bool isInBackgroundMode = false;

        /// <summary>
        /// Inicializa el objeto de aplicación Singleton. Esta es la primera línea de código creado
        /// ejecutado y, como tal, es el equivalente lógico de main() o WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();

            this.Suspending += OnSuspending;
            this.Resuming += App_Resuming;

            MemoryManager.AppMemoryUsageLimitChanging += MemoryManager_AppMemoryUsageLimitChanging;
            MemoryManager.AppMemoryUsageIncreased += MemoryManager_AppMemoryUsageIncreased;

            this.EnteredBackground += App_EnteredBackground;
            this.LeavingBackground += App_LeavingBackground;

            if (SettingsService.Instance.InicioConSistema)
            {
                runBack();
            } 
        }

        private async void runBack()
        {
            IList<AppDiagnosticInfo> infos = await AppDiagnosticInfo.RequestInfoForAppAsync();

            IList<AppResourceGroupInfo> appResources = infos[0].GetResourceGroups();

            await appResources[0].StartSuspendAsync();
        }

        private void App_Resuming(object sender, object e)
        {
            ToastService.ShowToast("Nasa API", "Rsuming");
        }

        private void App_LeavingBackground(object sender, LeavingBackgroundEventArgs e)
        {
            ToastService.ShowToast("Nasa API", "Leaving background");

            isInBackgroundMode = false;

            if (Window.Current.Content == null)
            {
                ToastService.ShowToast("Nasa API", "Loading view");

                // Create root frame
                CreateRootFrame(ApplicationExecutionState.Running, string.Empty);
            }
        }

        private void App_EnteredBackground(object sender, EnteredBackgroundEventArgs e)
        {
            ToastService.ShowToast("Nasa API", "Entered background");

            isInBackgroundMode = true;
        }

        private void MemoryManager_AppMemoryUsageIncreased(object sender, object e)
        {
            ToastService.ShowToast("Nasa API", "Memory usage increased");

            var level = MemoryManager.AppMemoryUsageLevel;

            if (level == AppMemoryUsageLevel.OverLimit || level == AppMemoryUsageLevel.High)
            {
                ReduceMemoryUsage(MemoryManager.AppMemoryUsageLimit);
            }
        }

        private void ReduceMemoryUsage(ulong appMemoryUsageLimit)
        {
            if (isInBackgroundMode && Window.Current.Content != null)
            {
                ToastService.ShowToast("Nasa API", "Unloading view");

                Window.Current.Content = null;
            }

            GC.Collect();
        }

        private void MemoryManager_AppMemoryUsageLimitChanging(object sender, AppMemoryUsageLimitChangingEventArgs e)
        {
            ToastService.ShowToast("Nasa API", "Memory usage limit changing from "
                + (e.OldLimit / 1024) + "K to "
                + (e.NewLimit / 1024) + "K");

            if (MemoryManager.AppMemoryUsage >= e.NewLimit)
            {
                ReduceMemoryUsage(e.NewLimit);
            }
        }

        /// <summary>
        /// Se invoca cuando la aplicación la inicia normalmente el usuario final. Se usarán otros puntos
        /// de entrada cuando la aplicación se inicie para abrir un archivo específico, por ejemplo.
        /// </summary>
        /// <param name="e">Información detallada acerca de la solicitud y el proceso de inicio.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            CreateRootFrame(e.PreviousExecutionState, e.Arguments);

            // Asegurarse de que la ventana actual está activa.
            Window.Current.Activate();
        }

        private void CreateRootFrame(ApplicationExecutionState previousExecutionState, string arguments)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null)
            {
                //ShowToast("Creating Frame");

                rootFrame = new Frame();

                // Set the default language
                //rootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];

                rootFrame.NavigationFailed += OnNavigationFailed;

                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                rootFrame.Navigate(typeof(MainPage), arguments);
            }
        }

        /// <summary>
        /// Se invoca cuando la aplicación la inicia normalmente el usuario final. Se usarán otros puntos
        /// </summary>
        /// <param name="sender">Marco que produjo el error de navegación</param>
        /// <param name="e">Detalles sobre el error de navegación</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Se invoca al suspender la ejecución de la aplicación. El estado de la aplicación se guarda
        /// sin saber si la aplicación se terminará o se reanudará con el contenido
        /// de la memoria aún intacto.
        /// </summary>
        /// <param name="sender">Origen de la solicitud de suspensión.</param>
        /// <param name="e">Detalles sobre la solicitud de suspensión.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Guardar el estado de la aplicación y detener toda actividad en segundo plano
            deferral.Complete();
        }
    }
}
