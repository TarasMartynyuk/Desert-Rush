using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour 
{

	[SerializeField] AudioClip pickupSFX;
	[SerializeField] protected Sprite itemSprite;
	AudioSource audioS;

	protected static bool fuelTipShown; 

	
	void Awake()
	{
		audioS = Camera.main.GetComponent<AudioSource>();
		if(audioS == null)
		{
			Debug.LogError("Item cant find audioSource");
		}
	}
	public virtual void OnPickup(Player player)
	{
		
		audioS.PlayOneShot(pickupSFX);
	}
	

	public abstract void OnVehicleInteraction(Vehicle vehicle);
	
	
}
