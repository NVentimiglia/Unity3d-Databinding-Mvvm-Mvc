// -------------------------------------
//  Domain		: Avariceonline.com
//  Author		: Nicholas Ventimiglia
//  Product		: Unity3d Foundation
//  Published		: 2015
//  -------------------------------------
using Foundation.Databinding.View;
using UnityEngine;

namespace Foundation.Databinding.Components
{
    /// <summary>
    /// Attach next to an AudioSource to let the AudioManager multiply the sources volume by its AudioLayer.
    /// </summary>
    [AddComponentMenu("Foundation/Databinding/AudioRegulator")]
    [RequireComponent(typeof(AudioSource))]
    public class AudioRegulator : MonoBehaviour
    {
        /// <summary>
        /// layer this is a member of
        /// </summary>
        public AudioLayer Layer;

        /// <summary>
        /// Default unmodified audio volume
        /// </summary>
        public float LocalVolume { get; set; }

        /// <summary>
        /// Audio source
        /// </summary>
        [HideInInspector]
        public AudioSource Audio;

        protected void Awake()
        {
            Audio = GetComponent<AudioSource>();

            LocalVolume = Audio.volume;

            UpdateVolume();

            AudioManager.OnVolumeChanged += AudioManager_OnVolumeChanged;

        }

        protected void OnDestroy()
        {
            AudioManager.OnVolumeChanged -= AudioManager_OnVolumeChanged;
        }

        void AudioManager_OnVolumeChanged(AudioLayer arg1, float arg2)
        {
            if (arg1 == Layer)
                UpdateVolume();
        }
        
        public void UpdateVolume()
        {
            Audio.volume = LocalVolume * AudioManager.GetVolume(Layer);
        }
    }
}