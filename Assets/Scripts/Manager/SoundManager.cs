using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoSingleton<SoundManager>
{
	private SortedList<string, AudioClip> mSounds = new SortedList<string, AudioClip>();
	private float mSFXVolume;
	private float mBGMVolume;
	private LinkedList<AudioSource> mSFXs = new LinkedList<AudioSource>();
	private AudioSource mBGM;

	private void Awake()
	{
		AudioClip[] Temp = Resources.LoadAll<AudioClip>("Sounds");
		for (int i = 0; i < Temp.Length; i++)
		{
			mSounds.Add(Temp[i].name, Temp[i]);
		}
		mBGMVolume = 1;
		mSFXVolume = 1;
	}

	private void Update()
	{
		LinkedListNode<AudioSource> iter = mSFXs.First;
		for (int i = 0; i < mSFXs.Count; i++)
		{
			if (iter.Value.isPlaying == false)
			{
				iter.Value.gameObject.SetActive(false);
				if (iter.Next != null)
				{
					iter = iter.Next;
					mSFXs.Remove(iter.Previous);
				}
				else
				{
					mSFXs.Remove(iter);
				}
			}
			else
			{
				iter = iter.Next;
			}
		}
	}

	public void PlaySound(string _Key, bool _IsBGM = false, bool _Loop = false)
	{
		AudioSource Temp;
		Temp = ObjectManager.Instance.AddObject("SFX", Vector3.zero).GetComponent<AudioSource>();
		Temp.clip = mSounds[_Key];
		Temp.Play();
		Temp.loop = _Loop;
		if (_IsBGM)
		{
			Temp.volume = mBGMVolume;
			mBGM = Temp;
		}
		else
		{
			Temp.volume = mSFXVolume;
			mSFXs.AddFirst(Temp);
		}
	}

	public void SetBGMVolume(float _Volume)
	{
		mBGMVolume = _Volume;
		if (mBGM != null)
		{
			mBGM.volume = _Volume;
		}
	}

	public float GetBGMVolume()
	{
		return mBGMVolume;
	}

	public void SetSFXVolume(float _Volume)
	{
		mSFXVolume = _Volume;
		foreach (var iter in mSFXs)
		{
			iter.volume = _Volume;
		}
	}

	public float GetSFXVolume()
	{
		return mSFXVolume;
	}

	public void PauseAll()
	{
		foreach (var iter in mSFXs)
		{
			iter.Pause();
		}
	}

	public void ResumeAll()
	{
		foreach (var iter in mSFXs)
		{
			iter.UnPause();
		}
	}
}
