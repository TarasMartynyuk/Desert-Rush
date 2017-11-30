using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItem : Item
{
	[SerializeField] GameObject weaponPrefab;

	public override void OnPickup(Player player)
	{
		base.OnPickup(player);

		GameObject newWeapon = Instantiate(weaponPrefab, player.transform);
		player.AddWeapon(newWeapon);
		Destroy(gameObject);

		UIController.Instance.SpawnTipAtPos(transform.position + Vector3.left * 10f + Vector3.up * 5f, 
			"Now this plasma looks like a decent answer to those raiders with assalut rifles! Press 'F' to cycle through your weapons" 
			, 0.8f);
	}

    public override void OnVehicleInteraction(Vehicle vehicle)
    {
        throw new NotImplementedException();
    }
}
