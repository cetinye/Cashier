using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Collections;
using UnityEngine.Audio;

namespace Cashier
{
	public class AudioManager : MonoBehaviour
	{
		public static AudioManager instance;
		public List<Sound> sounds = new List<Sound>();

		void Awake()
		{
			instance = this;

			foreach (Sound s in sounds)
			{
				s.source = gameObject.AddComponent<AudioSource>();
				s.source.clip = s.clip;
				s.source.volume = s.volume;
				s.source.loop = s.loop;
			}
		}

		public void Play(SoundType name)
		{
			Sound sound = sounds.Find(sound => sound.name == name);
			sound.source.Play();
		}

		public void PlayOneShot(SoundType name)
		{
			Sound sound = sounds.Find(sound => sound.name == name);
			sound.source.PlayOneShot(sound.clip);
		}

		public void PlayIf(SoundType name)
		{
			Sound sound = sounds.Find(sound => sound.name == name);
			if (!sound.source.isPlaying)
				sound.source.Play();
		}

		public void PlayAt(SoundType name, float startTime)
		{
			Sound sound = sounds.Find(sound => sound.name == name);
			sound.source.time = startTime;
			sound.source.Play();
		}

		public void Stop(SoundType name)
		{
			Sound sound = sounds.Find(sound => sound.name == name);
			sound.source.Stop();
		}

		public AudioSource GetSoundSource(SoundType name)
		{
			Sound sound = sounds.Find(sound => sound.name == name);
			return sound.source;
		}

		public void FadeTo(SoundType name, float target, float time)
		{
			Sound sound = sounds.Find(sound => sound.name == name);
			float initialVolume = 1f;
			sound.source.DOFade(target, time).OnComplete(() =>
			{
				sound.source.Stop();
				sound.source.volume = initialVolume;
			});
		}

		public void PlayAfterXSeconds(SoundType name, float timeToWait)
		{
			StartCoroutine(DelayedPlay(name, timeToWait));
		}

		IEnumerator DelayedPlay(SoundType name, float timeToWait)
		{
			yield return new WaitForSeconds(timeToWait);
			Sound sound = sounds.Find(sound => sound.name == name);
			sound.source.Play();
		}
	}

	public enum SoundType
	{
		Background,
		Barcode,
		Correct,
		CorrectOnPanel,
		OneNumber,
		TwoNumber,
		ThreeNumber,
		Wrong,
		TimesUp,
	}

	[System.Serializable]
	public class Sound
	{
		public SoundType name;
		public AudioClip clip;

		[Range(0f, 1f)]
		public float volume;

		public bool loop;

		[HideInInspector]
		public AudioSource source;
	}
}