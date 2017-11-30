using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusicPlayer : MonoBehaviour
{
	public AudioClip music1, music2;
	[SerializeField] AudioSource audioS;
	
	
	void Awake()
	{
		audioS.clip = music1;
		
		audioS.Play();
	}

	public void Resume()
	{
		if(audioS.isPlaying == false)
			audioS.Play();
			
	}

	public void Pause()
	{
		audioS.Pause();
	}
	
	private void changeClip()
	{
		audioS.clip = music1 == audioS.clip ? music2 : music1;
	}

	
}
