using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;

namespace NasaAPI.services
{
    class SettingsService
    {
        static SettingsService instance;

        public static SettingsService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SettingsService();
                }

                return instance;
            }
        }

        const string InicioConSistemaKey = "inicio-con-sistema";
        const string ActualizacionAutomaticaKey = "actualizacion-automatica";
        const string MostrarNotificacionesKey = "mostrar-notificaciones";

        IPropertySet settings = ApplicationData.Current.RoamingSettings.Values;

        public bool InicioConSistema
        {
            get
            {
                object setting;

                if (settings.TryGetValue(InicioConSistemaKey, out setting))
                {
                    return (bool)setting;
                }
                return false;
            }
            set
            {
                settings[InicioConSistemaKey] = value;

                EjecutarConSistema(value);
            }
        }

        public bool ActualizacionAutomatica
        {
            get
            {
                object setting;

                if (settings.TryGetValue(ActualizacionAutomaticaKey, out setting))
                {
                    return (bool)setting;
                }
                return false;
            }
            set
            {
                if (ActualizacionAutomatica != value)
                {
                    EjecutarConSistema(value);
                }

                settings[ActualizacionAutomaticaKey] = value;
            }
        }

        public bool MostrarNotificaciones
        {
            get
            {
                object setting;

                if (settings.TryGetValue(MostrarNotificacionesKey, out setting))
                {
                    return (bool)setting;
                }
                return false;
            }
            set
            {
                settings[MostrarNotificacionesKey] = value;
            }
        }

        private async void EjecutarConSistema(bool value)
        {
            StartupTask startupTask = await StartupTask.GetAsync("NasaAPIID");

            if (value)
            {
                switch (startupTask.State)
                {
                    case StartupTaskState.Disabled:
                        // Task is disabled but can be enabled.

                        StartupTaskState newState = await startupTask.RequestEnableAsync();

                        ToastService.ShowToast("NASA API", newState.ToString());

                        break;

                    case StartupTaskState.DisabledByUser:
                        // Task is disabled and user must enable it manually.

                        if (value)
                        {
                            MessageDialog dialog1 = new MessageDialog("Debe habilitar el inicio automatico desde el administrador de tareas.");

                            await dialog1.ShowAsync();
                        }

                        break;

                    case StartupTaskState.DisabledByPolicy:

                        MessageDialog dialog2 = new MessageDialog("No es posible habilitar el inicio automatico.");

                        await dialog2.ShowAsync();

                        break;

                    //case StartupTaskState.Enabled:
                        
                    //    MessageDialog dialog3 = new MessageDialog("El inicio automatico ya se encuentra habilitado.");

                    //    await dialog3.ShowAsync();

                    //    break;
                }
            }
            else
            {
                startupTask.Disable();
            }

            
        }
    }
}
