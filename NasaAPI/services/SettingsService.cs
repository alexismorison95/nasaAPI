using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation.Collections;
using Windows.Storage;

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

        //private async void EjecutarConSistema()
        //{
        //    StartupTask startupTask = await StartupTask.GetAsync(StartupTask.);

            
        //}
    }
}
