using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Data.Interfaces
{
    /// <summary>
    /// Would be invoked by the player to request the Items from the provider
    /// </summary>
    public interface IMusicProvider
    {
        void QueryMusic(QueryType queryType, string query, Action<List<IMusicItem>> response);
        Uri GetThumbnailUri(string Id, ThumbnailSize size = ThumbnailSize.MoviePoster);
        int GetQualityIdentifier(Quality quality);
    }
}
