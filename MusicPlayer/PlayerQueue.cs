using Microsoft.Phone.BackgroundAudio;
using MusicPlayer.Data.Interfaces;
using PlayStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamPlayer
{
    public enum TrackOrder
    {
        Current,
        Next,
        Previous
    }

    public sealed class PlayerQueue
    {
        private List<IMusicItem> items = new List<IMusicItem>();
        private static readonly PlayerQueue instance = new PlayerQueue();
        private int index = 0;
        public static PlayerQueue GetInstance()
        {
            return instance;
        }

        private PlayerQueue() { }

        void Clear()
        {
            items.Clear();
        }

        public void Add(IMusicItem item)
        {
            if (items.Count == 0)
            {
                //track item = 0;
                index = 0;
            }

            items.Add(item);
        }

        public void Remove(IMusicItem item)
        {
            items.Remove(item);
        }

        public void GetTrack(TrackOrder order, Action<IMusicItem> onAvailable)
        {
            if (items.Count == 0)
            {
                onAvailable(null);
                return;
            }

            int currentIndex = index;
            switch (order)
            {
                case TrackOrder.Previous:
                    currentIndex = index - 1;
                    break;
                case TrackOrder.Next:
                    currentIndex = index + 1;
                    break;
                case TrackOrder.Current:
                    currentIndex = index;
                    break;
                default:
                    break;
            }

            if (currentIndex > items.Count - 1)
            {
                currentIndex = 0;
            }
            else if (currentIndex < 0 && items.Count > 0)
            {
                currentIndex = items.Count - 1;
            }

            items[currentIndex].GetMusicItemUri((ex, playerItem) =>
            {
                if (ex == null)
                {
                    index = currentIndex;
                    onAvailable(playerItem);
                }
                else
                {
                    onAvailable(null);
                }
            });
        }

        public List<IMusicItem> PlayerQueueItems
        {
            get
            {
                return items;
            }
        }
    }
}
