using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : Weapon
{
    protected override void OnReadyToFire(Vector3 target, string tagToHit)
    {
        //spawn Projectile
		GameObject proj = Instantiate(projectile, projectileSpawnPosition.transform.position, 
			Quaternion.LookRotation(Vector3.forward, target - transform.position));

		firedProjs.Add(proj);
		
		// show fire animation
		ShowFireEffect();

		//move projectile in the direction towards the target
        Projectile projScript = proj.GetComponent<Projectile>();
		projScript.LaunchLinear(proj, target, tagToHit, shooterCol, fireEffectDuration);
    }
}
