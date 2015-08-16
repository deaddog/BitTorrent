using System;

namespace BitTorrent
{
    /// <summary>
    /// Describes the current state of a torrent as reported by an <see cref="BitTorrent.API.IClient"/>.
    /// Note that multiple values can be applied (as flags) for a torrent.
    /// Additionally these values must be viewed in conjunction with the torrents <see cref="ActiveStates"/> state.
    /// </summary>
    [Flags]
    public enum DownloadStates
    {
        /// <summary>
        /// The client is currently checking the torrent.
        /// </summary>
        Checking,
        /// <summary>
        /// The torrent is queued for download or seeding. Check the <see cref="Seeding"/> and <see cref="Downloading"/> flags.
        /// </summary>
        Queued,
        /// <summary>
        /// The client is currently seeding the torrent.
        /// </summary>
        Seeding,
        /// <summary>
        /// The client is currently downloading the torrent.
        /// </summary>
        Downloading,
        /// <summary>
        /// The client has reported an error for the torrent. This will cause the torrent to have an <see cref="ActiveStates.Stopped"/> state.
        /// </summary>
        Error
    }
}
