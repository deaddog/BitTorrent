namespace BitTorrent
{
    /// <summary>
    /// Describes the are user-controlled part of a torrents state.
    /// Details about the state of the torrents progress are represented using <see cref="DownloadStates"/>.
    /// </summary>
    public enum ActiveStates
    {
        /// <summary>
        /// The torrent has been started by the user, though it might not be transferring data at this moment (see <see cref="DownloadStates"/>).
        /// </summary>
        Started,
        /// <summary>
        /// The torrent has been stopped by the user (or system); it is not transferring data and will not begin to do so without user interaction.
        /// Clients will return this state for torrents that are in an <see cref="DownloadStates.Error"/> state.
        /// </summary>
        Stopped
    }
}
