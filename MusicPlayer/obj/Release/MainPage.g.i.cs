﻿#pragma checksum "C:\streamingvideos\AudioPlayer\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "14D48D4185DE4C932E44299420757391"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34003
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace StreamPlayer {
    
    
    public partial class MainPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.StackPanel TitlePanel;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal System.Windows.Controls.TextBlock txtTrack;
        
        internal System.Windows.Controls.ProgressBar positionIndicator;
        
        internal System.Windows.Controls.TextBlock textPosition;
        
        internal System.Windows.Controls.TextBlock textRemaining;
        
        internal System.Windows.Controls.TextBox QueryText;
        
        internal System.Windows.Controls.TextBlock LoadingMessage;
        
        internal System.Windows.Controls.Image LoadingImage;
        
        internal System.Windows.Controls.Image loadingwait;
        
        internal System.Windows.Controls.Image BaseImage;
        
        internal Microsoft.Phone.Controls.LongListSelector Queue;
        
        internal System.Windows.Controls.Button Cache;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/StreamPlayer;component/MainPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.TitlePanel = ((System.Windows.Controls.StackPanel)(this.FindName("TitlePanel")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.txtTrack = ((System.Windows.Controls.TextBlock)(this.FindName("txtTrack")));
            this.positionIndicator = ((System.Windows.Controls.ProgressBar)(this.FindName("positionIndicator")));
            this.textPosition = ((System.Windows.Controls.TextBlock)(this.FindName("textPosition")));
            this.textRemaining = ((System.Windows.Controls.TextBlock)(this.FindName("textRemaining")));
            this.QueryText = ((System.Windows.Controls.TextBox)(this.FindName("QueryText")));
            this.LoadingMessage = ((System.Windows.Controls.TextBlock)(this.FindName("LoadingMessage")));
            this.LoadingImage = ((System.Windows.Controls.Image)(this.FindName("LoadingImage")));
            this.loadingwait = ((System.Windows.Controls.Image)(this.FindName("loadingwait")));
            this.BaseImage = ((System.Windows.Controls.Image)(this.FindName("BaseImage")));
            this.Queue = ((Microsoft.Phone.Controls.LongListSelector)(this.FindName("Queue")));
            this.Cache = ((System.Windows.Controls.Button)(this.FindName("Cache")));
        }
    }
}

