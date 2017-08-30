using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoutWeapon : Weapon
{
	[SerializeField] GameObject projectileSpawnPositionAux;
    [SerializeField] GameObject fireEffectAux;

	bool mainSpawnPosTurn = true;
	
    protected override void OnReadyToFire(Vector3 target, string tagToHit)
    {
        // print(ProjectileStreak);

        Vector3 projectileSpawnPos = mainSpawnPosTurn ? projectileSpawnPosition.transform.position :
            projectileSpawnPositionAux.transform.position;

        //spawn Projectile
		GameObject proj = Instantiate(projectile, projectileSpawnPos, 
			Quaternion.LookRotation(Vector3.forward, target - transform.position));

        firedProjs.Add(proj);

        
		
		// show fire animation
        if(mainSpawnPosTurn)
        {
		    ShowFireEffect();
        }
        else
        {
            ShowFireEffectAux();
        }


		//move projectile in the direction towards the target
        Projectile projScript = proj.GetComponent<Projectile>();
		projScript.LaunchLinear(proj, target, tagToHit, shooterCol, fireEffectDuration);

        mainSpawnPosTurn = !mainSpawnPosTurn;
    }

    protected void ShowFireEffectAux()
	{
		StartCoroutine(ShowFireEffectAuxCoroutine());
	}

	private IEnumerator ShowFireEffectAuxCoroutine()
	{
		fireEffectAux.SetActive(true);

		yield return GameController.Instance.Pause(fireEffectDuration);

		fireEffectAux.SetActive(false);
		
	}
}
