﻿#pragma checksum "D:\Programacion\C#\nasaAPI\NasaAPI\NasaAPI\Pages\Home.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "BBF75D0C6FFCE701436C2636F834D0E0"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NasaAPI.Pages
{
    partial class Home : 
        global::Windows.UI.Xaml.Controls.Page, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {
        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 10.0.18362.1")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 2: // Pages\Home.xaml line 70
                {
                    this.progressImage2 = (global::Windows.UI.Xaml.Controls.ProgressRing)(target);
                }
                break;
            case 3: // Pages\Home.xaml line 74
                {
                    this.textBlockWallpaper = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 4: // Pages\Home.xaml line 79
                {
                    this.btnGuardarComo = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.btnGuardarComo).Click += this.Button_Click_1;
                }
                break;
            case 5: // Pages\Home.xaml line 85
                {
                    this.btnAplicar = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.btnAplicar).Click += this.Button_Click;
                }
                break;
            case 6: // Pages\Home.xaml line 57
                {
                    this.textBlockDescripcion = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 7: // Pages\Home.xaml line 30
                {
                    this.textBlockTitulo = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 8: // Pages\Home.xaml line 40
                {
                    this.progressImage = (global::Windows.UI.Xaml.Controls.ProgressRing)(target);
                }
                break;
            case 9: // Pages\Home.xaml line 43
                {
                    this.textBlockAutor = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 10: // Pages\Home.xaml line 35
                {
                    this.imageVista = (global::Windows.UI.Xaml.Controls.Image)(target);
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        /// <summary>
        /// GetBindingConnector(int connectionId, object target)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 10.0.18362.1")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Windows.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Windows.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}

