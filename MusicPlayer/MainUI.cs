using MusicPlayer.Data.Interfaces;
using PlayStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MusicPlayer.Shell;
using MusicPlayer.Data.YouTube;
using System.Threading;

namespace StreamPlayer
{
    public partial class MainUI
    {
        private const string seperatorSearchQuery = "+";
        private const char seperatorSpace = ' ';
        private Dictionary<ProviderType, IMusicProvider> providers = new Dictionary<ProviderType, IMusicProvider>();

        private void RegisterProviders()
        {
            //as of now only one is registered, we can enable multiple
            providers.Add(ProviderType.YouTube, new YouTubeProvider());
        }

        private IMusicProvider[] GetProviders()
        {
            return providers.Values.ToArray<IMusicProvider>();
        }

        private StringBuilder PrepareSearchQuery(string searchRawText)
        {
            StringBuilder builder = new StringBuilder();
            try
            {
                if (!string.IsNullOrWhiteSpace(searchRawText))
                {
                    string[] queryElements = searchRawText.Split(seperatorSpace);
                    if (queryElements != null && queryElements.Length > 0)
                    {
                        builder.Append(queryElements[0]);

                        for (var index = 1; index < queryElements.Length; index++)
                        {
                            builder.Append(seperatorSearchQuery);
                            builder.Append(queryElements[index]);
                        }
                    }
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                Logger.LogException(ex);
            }

            return builder;
        }

        private void Search(string searchText, Action<List<IMusicItem>> searchItems)
        {
            try
            {
                int dataAvailableCount = 0;
                var providers = GetProviders();
                int count = providers.Length;
                List<IMusicItem> aggregatedMusicItems = new List<IMusicItem>();
                foreach (var provider in providers)
                {
                    provider.QueryMusic(QueryType.GENERAL, searchText, new Action<List<IMusicItem>>((obj) =>
                        {
                            aggregatedMusicItems.Concat<IMusicItem>(obj);
                            dataAvailableCount++;

                            if (dataAvailableCount == count)
                            {
                                searchItems(aggregatedMusicItems);
                            }
                        }
                   ));
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        internal void Navigate(Uri uri)
        {
            NavigationService.Navigate(new Uri("/PlayerUI.xaml", UriKind.Relative));
        }
    }
}
