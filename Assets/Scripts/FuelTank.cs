using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelTank : Item 
{

	int fuelSupply = 100;

	public override void OnPickup(Player player)
	{
		if(fuelTipShown == false)
			{
				UIController.Instance.SpawnTipAtPos(transform.position + Vector3.left * 7f + Vector3.up * 8f, "Good, now take it to the buggy " +
				"and press 'E' to refill its tank", 0.6f);
				fuelTipShown = true;
			}

		base.OnPickup(player);
		
		player.AddItem(this);
		UIController.Instance.AddIconToInventory(this, itemSprite);
		Destroy(gameObject);
	}

    public override void OnVehicleInteraction(Vehicle vehicle)
    {
        vehicle.ReplenishFuel(fuelSupply);
		UIController.Instance.RemoveItemIconFromInvemntory(this);
    }
}
