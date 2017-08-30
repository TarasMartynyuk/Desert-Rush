using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scout : Enemy {

	
	//he s shooting is time - based
	[SerializeField] float attackCoolDown;
	[SerializeField] float movementPause;
	// [SerializeField] float minDist;
	
	
	float timeSinceLastAttack;
	
	public override void AttackTarget()
	{
		//this fucker shoots and runs simultaniously
		//check if target is within fire area
		if(Vector2.Distance(transform.position, target.transform.position) <= fireDistance)
		{
			//shooting part
			// print("Shooting at " + target.name);
			//fire a queue of *projectilesPerAttack* projectiles
			if(shooting)
			{
				weapon.AttemptToFire(target.transform.position, "Player");

				if(weapon.ProjectileStreak >= projectilesPerAttack)
				{
					shooting = false;
					
				}
			}

			//moving part
			if(moving == false)
			{
					
					StartCoroutine(MoveInDirectionTowardsTarget());
					moving = true;
					
					
			}
		}
		else
		{
			

			if(moving == false)
			{
				MoveCloserToTarget();
			}
			
		}

		//update shooting status - in both variants
		if(shooting == false)
		{
			timeSinceLastAttack += Time.deltaTime;
			if(timeSinceLastAttack >= attackCoolDown)
			{
				shooting = true;
				timeSinceLastAttack = 0f;
			}
		}
	}

	IEnumerator MoveInDirectionTowardsTarget()
	{
		Vector3 direction = GetRandomDirectionTowardsPlayer(true);
		yield return StartCoroutine(MoveInDirection(direction, moveTime));

		//stand still for some time
		moving = true;
		yield return GameController.Instance.Pause(movementPause);
		moving = false;
	}

	
}
