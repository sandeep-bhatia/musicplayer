/* 
    Copyright (c) 2011 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Phone.Controls;
using Microsoft.Phone.BackgroundAudio;
using Microsoft.Phone.Shell;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using PlayStation;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using MusicPlayer.Data.Interfaces;

namespace StreamPlayer
{
    public partial class PlayerUI : PhoneApplicationPage
    {
        // Timer for updating the UI
        DispatcherTimer _timer;
        // Indexes into the array of ApplicationBar.Buttons
        const int prevButton = 0;
        const int playButton = 1;
        const int pauseButton = 2;
        const int nextButton = 3;

        // Constructor
        public PlayerUI()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(PlayerUI_Loaded);
            UpdateButtons(false, true, false, false);
        }

        /// <summary>
        /// PlayerUI_Loaded main load page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PlayerUI_Loaded(object sender, RoutedEventArgs e)
        {
            // Initialize a timer to update the UI every half-second.
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(0.5);
            Queue.ItemsSource = PlayerQueue.GetInstance().PlayerQueueItems;

            BackgroundAudioPlayer.Instance.PlayStateChanged += new EventHandler(Instance_PlayStateChanged);
            if (BackgroundAudioPlayer.Instance.PlayerState == PlayState.Playing)
            {
                // If audio was already playing when the app was launched, update the UI.
                UpdateButtons(true, false, true, true);
            }
        }


        /// <summary>
        /// PlayStateChanged event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Instance_PlayStateChanged(object sender, EventArgs e)
        {
            switch (BackgroundAudioPlayer.Instance.PlayerState)
            {
                case PlayState.Playing:
                    UpdateButtons(true, false, true, true);
                    // Start the timer for updating the UI.
                    _timer.Start();
                    break;
                case PlayState.Paused:
                    // Update the UI.
                    UpdateButtons(true, true, false, true);
                    // Stop the timer for updating the UI.
                    _timer.Stop();
                    break;
                case PlayState.TrackEnded:
                    PlayNextTrack();
                    break;
                case PlayState.TrackReady:
                    UpdateButtons(true, true, false, true);
                    break;
                case PlayState.BufferingStarted:
                    UpdateButtons(false, false, false, false);
                    break;
            }
        }


        private void PlayTrack(int index)
        {
            GetTrack(TrackOrder.Current, new Action<IMusicItem>((item) =>
            {
                BackgroundAudioPlayer.Instance.Play();
                SetCurrentPlayerItemView(item);
            }));
        }

        private void PlayNextTrack()
        {
            GetTrack(TrackOrder.Next, new Action<IMusicItem>((item) =>
            {
                BackgroundAudioPlayer.Instance.Play();
                SetCurrentPlayerItemView(item);
            }));
        }

        /// <summary>
        /// Helper method to update the state of the ApplicationBar.Buttons
        /// </summary>
        /// <param name="prevBtnEnabled"></param>
        /// <param name="playBtnEnabled"></param>
        /// <param name="pauseBtnEnabled"></param>
        /// <param name="nextBtnEnabled"></param>
        void UpdateButtons(bool prevBtnEnabled, bool playBtnEnabled, bool pauseBtnEnabled, bool nextBtnEnabled)
        {
            // Set the IsEnabled state of the ApplicationBar.Buttons array
            ((ApplicationBarIconButton)(ApplicationBar.Buttons[prevButton])).IsEnabled = prevBtnEnabled;
            ((ApplicationBarIconButton)(ApplicationBar.Buttons[playButton])).IsEnabled = playBtnEnabled;
            ((ApplicationBarIconButton)(ApplicationBar.Buttons[pauseButton])).IsEnabled = pauseBtnEnabled;
            ((ApplicationBarIconButton)(ApplicationBar.Buttons[nextButton])).IsEnabled = nextBtnEnabled;
        }

        /// <summary>
        /// Click handler for the Skip Previous button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void prevButton_Click(object sender, EventArgs e)
        {
            // Disable the button so the user can't click it multiple times before 
            // the background audio agent is able to handle their request.
            ((ApplicationBarIconButton)(ApplicationBar.Buttons[prevButton])).IsEnabled = false;
            GetTrack(TrackOrder.Previous, new Action<IMusicItem>((item) =>
            {
                BackgroundAudioPlayer.Instance.SkipPrevious();
                SetCurrentPlayerItemView(item);
            }));
            // Disable the button so the user can't click it multiple times before 
            // the background audio agent is able to handle their request.
            ((ApplicationBarIconButton)(ApplicationBar.Buttons[prevButton])).IsEnabled = true;
        }


        /// <summary>
        /// Click handler for the Play button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void playButton_Click(object sender, EventArgs e)
        {
            GetTrack(TrackOrder.Current, new Action<IMusicItem>((item) =>
            {
                BackgroundAudioPlayer.Instance.Play();
                SetCurrentPlayerItemView(item);
            }));
        }


        /// <summary>
        /// Click handler for the Pause button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pauseButton_Click(object sender, EventArgs e)
        {
            // Tell the backgound audio agent to pause the current track.
            BackgroundAudioPlayer.Instance.Pause();
        }


        /// <summary>
        /// Click handler for the Skip Next button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nextButton_Click(object sender, EventArgs e)
        {
            // Show the indeterminate progress bar.
            // the background audio agent is able to handle their request.
            ((ApplicationBarIconButton)(ApplicationBar.Buttons[nextButton])).IsEnabled = false;

            try
            {
                GetTrack(TrackOrder.Next, new Action<IMusicItem>((item) =>
                {
                    BackgroundAudioPlayer.Instance.Play();
                    SetCurrentPlayerItemView(item);
                }));
            }
            finally
            {
                ((ApplicationBarIconButton)(ApplicationBar.Buttons[nextButton])).IsEnabled = true;
            }
        }

        private void SetCurrentPlayerItemView(IMusicItem item)
        {
            currentItemTitle.Text = item.Title;
            BitmapImage image = new BitmapImage(new Uri(item.ThumbnailUrl));
            currentItemImage.Source = image;
        }

        private void GetTrack(TrackOrder trackOrder, Action<IMusicItem> T)
        {
            var dispatcher = Application.Current.RootVisual.Dispatcher;
            PlayerQueue.GetInstance().GetTrack(trackOrder, (item) =>
            {
                dispatcher.BeginInvoke(new Action(() =>
               {
                   BackgroundAudioPlayer.Instance.Track = item.Track;
                   T(item);
               }));
            });
        }
    }
}
