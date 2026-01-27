using UnityEngine;

namespace Masquerade.Models
{
    /// <summary>
    /// Class used to store data for a music audio clips. 
    /// Can be used in parts to create songs with different sections.
    /// </summary>
    public class SongData
    {
        public SongData(AudioClip clip, string name, string album, bool loop = true)
        {
            Clip = clip;
            Name = name;
            Loop = loop;
            Album = album;
        }   

        public AudioClip Clip { get; set; }
        public string Name { get; set; }

        public bool Loop { get; set; }

        public string Album { get; set; }
    }
}
