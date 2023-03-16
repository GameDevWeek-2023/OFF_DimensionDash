using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
	public class SoundManager : MonoBehaviour
	{

		public float fadeTime;


		[SerializeField]
		private Slider musicVolumeSlider;
		[SerializeField]
		private Slider effectVolumeSlider;
		[SerializeField]
		private List<AudioSource> musicSourceList;
		[SerializeField]
		private AudioSource effectSource;


		private float maxMusicVolume;
		private int   currentDimensionMusic;
		private int   priorDimensionMusic;
		private bool  fadeOut;
		private bool  fadeIn;
	


		void Start()
		{
			maxMusicVolume = musicVolumeSlider.value;
			foreach (AudioSource audioSource in musicSourceList) 
			{
				audioSource.volume    = 0;
				currentDimensionMusic = 0;
				fadeIn                = true;
			}
		}

		void Update()
		{
			FadeMusic();
		}

		public void DimensionChangeMusicSwap(int i) 
		{
			priorDimensionMusic   = currentDimensionMusic;
			currentDimensionMusic = i;
			fadeOut               = true;
			fadeIn                = true;
		}



		private void FadeMusic() 
		{
			if (fadeOut && musicSourceList[priorDimensionMusic] != null) {
				if (musicSourceList[priorDimensionMusic].volume > 0)
					musicSourceList[priorDimensionMusic].volume -= fadeTime;
				if (musicSourceList[priorDimensionMusic].volume <= 0) 
				{
					musicSourceList[priorDimensionMusic].volume = 0;
					fadeOut                                     = false;
				}
			}

			if (fadeIn) {
				if (musicSourceList[currentDimensionMusic].volume < maxMusicVolume)
					musicSourceList[currentDimensionMusic].volume += fadeTime;
				if (musicSourceList[currentDimensionMusic].volume >= maxMusicVolume) 
				{
					musicSourceList[currentDimensionMusic].volume = maxMusicVolume;
					fadeIn                                        = false;
				}
			}
		}


		public void ChangeMusicVolume() 
		{
			float newVolume = musicVolumeSlider.value;
			musicSourceList[currentDimensionMusic].volume = newVolume;
			maxMusicVolume                                = newVolume;
			Debug.Log(maxMusicVolume);
		}

		public void ChangeEffectVolume() {
			float newVolume = musicVolumeSlider.value;
			effectSource.volume = newVolume;
		}
	}
}
