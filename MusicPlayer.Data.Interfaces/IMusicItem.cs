using Microsoft.Phone.BackgroundAudio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Data.Interfaces
{
    /// <summary>
    /// All music item providers should implement this interface to work with the player
    /// </summary>
    public interface IMusicItem
    {
        /// <summary>
        /// Get the Audio Track for this music item
        /// </summary>
        AudioTrack Track { get; }

        /// <summary>
        /// The method that would ensure Music Item is requested
        /// </summary>
        /// <param name="callback"></param>
        void GetMusicItemUri(Action<Exception, IMusicItem> callback);

        /// <summary>
        /// Download this music item using this method
        /// </summary>
        void Download();

        /// <summary>
        /// Unique Id representing the music item
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Duration of this music item
        /// </summary>
        string DurationSecs { get; }

        /// <summary>
        /// Title that would be displayed for this music item
        /// </summary>
        string Title { get; }

        /// <summary>
        /// The thumbnail displayed for the music item
        /// </summary>
        string ThumbnailUrl { get; }

        /// <summary>
        /// Any Description that is associated with the music item
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Actor, Singer Information etc on the music item
        /// </summary>
        List<string> Actors { get; }

        /// <summary>
        /// When the last this item was updated
        /// </summary>
        string LastUpdated { get; }

        /// <summary>
        /// Is this music item in local download, is it serviced from local cache
        /// </summary>
        bool IsDownloaded { get; }

        /// <summary>
        /// Access Url is the final url from where the item is played
        /// </summary>
        string AccessUrl { get; }

        /// <summary>
        /// The initial url that can be used to request the item, this could be same as access url or different
        /// </summary>
        string RequestUrl { get; }
    }
}
