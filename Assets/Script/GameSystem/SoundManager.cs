using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Script.GameSystem
{
    public class SoundManager : MonoBehaviour
    {
        public AudioSource fxSource;
        public AudioSource playerSource;
        
        public static SoundManager Instance = null;
        
        // Start is called before the first frame update
        void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }else if(Instance != this)
            {
                Destroy(gameObject);
            }
        }

        public void PlayEnvironmentClip(AudioClip clip)
        {
            fxSource.clip = clip;
            fxSource.Play();
        }

        public void PlayPlayerSound(AudioClip clip)
        {
            playerSource.clip = clip;
            playerSource.Play();
        }
        
    }
}
