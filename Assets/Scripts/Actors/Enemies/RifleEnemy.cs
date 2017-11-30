using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// this guy shoots and moves in succession
/// </summary>
public class RifleEnemy : Enemy
{
	//when false - its turn to shoot
	[SerializeField] float pauseBeforeAttack;
	// [SerializeField] float minDist;

	bool turnToMove = false;
    public override void AttackTarget()
    {
		//check if target is within fire area
        if(Vector2.Distance(transform.position, target.transform.position) <= fireDistance)
		{
			if(shooting || moving)
				return;
			
			if(turnToMove)
			{
				// if(Vector3.Distance(transform.position, target.transform.position) <= minDist)
				// {
				Vector3 direction = GetRandomDirectionTowardsPlayer(true);
				StartCoroutine(MoveInDirection(direction, moveTime));
				// }
			}
			else
			{
				//shoot
				PauseThenFireNProjectilesAtTarget(pauseBeforeAttack, projectilesPerAttack, "Player");
			}

			//pick the other activity
			turnToMove = !turnToMove;

		}
		else
		{
			//get closer
			MoveCloserToTarget();
		}

    }

	void PauseThenFireNProjectilesAtTarget(float pauseTime, int n, string tagToHit)
	{
		StartCoroutine(FireNProjectilesAtTargetCoroutine(pauseTime, n, tagToHit));
	}


	IEnumerator FireNProjectilesAtTargetCoroutine(float pauseTime, int n, string tagToHit)
	{
		shooting = true;
		yield return GameController.Instance.Pause(pauseTime);
		
		while(weapon.ProjectileStreak < n)
		{
			weapon.AttemptToFire(target.transform.position, tagToHit);
			yield return GameController.Instance.WaitForEndOfFrameSync();
		}

		shooting = false;
	}

	
	

	
}
