using Il2CppDarkTonic.MasterAudio;
using Il2CppZenject;
using static Il2CppDarkTonic.MasterAudio.MasterAudio;
using Il2Col = Il2CppSystem.Collections.Generic;

namespace Masquerade.Factories
{
    /// <summary>
    /// Placeholder class for creating DynamicSoundGroup objects that store audio data for asset bundles.
    /// </summary>
    /// <remarks>
    /// Calling this a "Factory" is a bit of a misnomer. This will need updated to be more extensible.
    /// </remarks>
    public class DynamicSoundGroupFactory
    {
        /// <summary>
        /// Creates the base DynamicSoundGroup for the modloader.
        /// </summary>
        public static DynamicSoundGroupCreator DefaultModdedGroup()
        {
            if (ProjectContext._instance == null) return null;
            var sounds = ProjectContext._instance._container.InstantiateComponentOnNewGameObject<DynamicSoundGroupCreator>();
            sounds.musicPlaylists = new Il2Col.List<Playlist>();
            foreach (var song in vsMLCore.CustomMusic)
            {
                Playlist playlist = new Playlist() { playlistName = song.Key.ToString() };
                foreach (var clip in song.Value)
                {
                    var setting = new MusicSetting() { clip = clip.Clip, songName = clip.Name, isLoop = clip.Loop, audLocation = AudioLocation.Clip };
                    playlist.MusicSettings.Add(setting);
                }

                sounds.musicPlaylists.Add(playlist);
            }

            return sounds;
        }
    }
}