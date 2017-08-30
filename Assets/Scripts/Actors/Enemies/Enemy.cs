using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


public abstract class Enemy : Actor 
{
	//stats
	
	[SerializeField] protected int projectilesPerAttack;
	[SerializeField] protected float fireDistance;
	
	
	//AI stuff
	[SerializeField] protected float playerAggroDistance;
	[SerializeField] protected float vehicleAggroDistance;
	//later this will be used in target choosing AI
	[SerializeField] protected GameObject player;
	[SerializeField] protected GameObject vehicle;
	
	//movement behaivour
	[SerializeField] protected float moveTime;
	
	//shooting support
	[SerializeField] protected Weapon weapon;
	protected GameObject target;
	protected bool shooting;


	protected 	bool moving;
	float distanceToPlayer, distanceToVehicle;
	float shieldRadius;

	//music
	AudioSource audioS;
	[SerializeField] AudioClip deathSound;
	
	
#region Unity methods	
	void Start()
	{
		player = GameController.Instance.Player;
		vehicle = GameController.Instance.Vehicle;
		
		target = vehicle;
		Assert.IsNotNull(weapon);
		Assert.IsNotNull(player);
		Assert.IsNotNull(vehicle);

		audioS = GetComponent<AudioSource>();
		if(audioS == null)
		{
			Debug.LogError("no audio for enemy");
		}

		shieldRadius = GameController.Instance.Shield.GetComponent<CircleCollider2D>().radius;
		
		
	}
	
	void Update()
	{
		if(GameController.Instance.CurrState != GameState.GAME)
			return;
			
		//rotate to look at player
		transform.rotation = Quaternion.LookRotation(Vector3.forward, 
			target.transform.position - transform.position);

		distanceToPlayer = Vector2.Distance(this.transform.position, player.transform.position);
		distanceToVehicle = Vector2.Distance(this.transform.position, vehicle.transform.position);

		//check if player is in the range of attack
		if(distanceToPlayer <= playerAggroDistance)
		{
			target = player;
		}
		//if not, check if vehicle is in the range of attack
		else if(distanceToVehicle <= vehicleAggroDistance)
		{
			target = vehicle;
		}
		// print("Targeting " + target.name);
		AttackTarget();
	}
#endregion
	
	

	public override void SufferDamage(int damage)
	{
		health -= damage;
		if(health <= 0)
		{
			GameController.Instance.SpawnBloodCloud(transform.position);
			audioS.PlayOneShot(deathSound);
			Destroy(gameObject);
		}
	}

    public abstract void AttackTarget();

	/// <summary>
	/// randomizes a direction from straight to left to straight to right
	/// (clockwise)
	/// </summary>
	/// <returns></returns>
	protected Vector3 GetRandomDirectionTowardsPlayer(bool flanking)
	{
		float x = Random.Range(-1f, 1f);
		float y;

		if(flanking)
		{
			//maximum degree - 45 - SPLENDID!
			//ensure that y is less than x(to move sideways more than forward) 
			y = Random.Range(0f, Mathf.Abs(x));
		}
		else
		{
			y = Random.Range(0f, 1f);
		}
		
		return new Vector3(x, y, 0f).normalized;
	}

	protected void MoveCloserToTarget()
	{
		Vector3 direction = target.transform.position - transform.position;
		direction.Normalize();

		RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 5f);
		if(hit.collider == null)
		{
			//get closer
			transform.position = Vector3.MoveTowards(
			transform.position, target.transform.position, speed * Time.deltaTime);
		}
		else
		{
			//try bupass obstacle
			//rotate direction vector by 90
			direction = Quaternion.AngleAxis(-90, Vector3.forward) * direction;

			transform.position = transform.position + direction * Time.deltaTime * speed;
		}
	}

	protected bool IsInsideShield()
	{
		return GameController.Instance.Shield.activeSelf && distanceToVehicle < shieldRadius;
	}
	
#region Courotines and methods to invoke them
	/// <summary>
	/// moves in specified direction for <paramref name="direction"> sec 
	/// </summary>
	/// <param name="direction"></param>
	/// <returns></returns>
	protected IEnumerator MoveInDirection(Vector3 direction, float moveTime)
	{
		bool changedDirection = false;
		moving = true;
		float timer = 0f;

		while(timer <= moveTime)
		{
			//check if touched shield
			if(IsInsideShield() && changedDirection == false)
			{
				direction = - direction;
				changedDirection = true;
			}

			transform.Translate(direction * speed * Time.deltaTime);
			timer += Time.deltaTime;
			yield return GameController.Instance.WaitForEndOfFrameSync();
		}

		moving = false;
	}
	
	
	
# endregion


}
