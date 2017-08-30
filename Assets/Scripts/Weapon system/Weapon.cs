using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class Weapon : MonoBehaviour
{
	//points for spawning effects
	[SerializeField] protected GameObject projectileSpawnPosition;
	
	[SerializeField] protected GameObject projectile;
	[SerializeField] protected GameObject fireEffect;
	public Sprite sprite;
	
	
	[SerializeField] protected float warmupTime;
	[SerializeField] protected float fireEffectDuration = 0.3f;
	[SerializeField] protected float timeBetweenShots;
	[SerializeField] AudioClip fireSound;
	AudioSource audioS;


	//number of projs fired in succession since last time weapon did not fire
	public int ProjectileStreak {get; protected set;}
	protected List<GameObject> firedProjs;
	protected Collider2D shooterCol;
	bool warmedUp = false;
	bool readytoFire = true;
	bool attemptedToFireThisFrame = false;
	float timeSinceWarmupStarted = 0f;
	float timeSinceLastShot = 0f;

#region Unity funcs	
	void Awake()
	{
		Assert.IsNotNull(projectileSpawnPosition);
		Assert.IsNotNull(projectile);
		Assert.IsNotNull(fireEffect);
		
		// Collider2D[] colliders = GetComponentsInParent<Collider2D>();

		// if(colliders.Length == 1)
		// {
		// 	shooterCol = colliders[0];
		// }

		// foreach (Collider2D col in colliders)
		// {
		// 	if(col.isTrigger)
		// 	{
		// 		shooterCol = col;
		// 	}
		// }
		shooterCol = GetComponentInParent<Collider2D>();

		if(shooterCol == null)
		{
			Debug.LogError("weapon cant find collider of its shooter!");
		}

		firedProjs = new List<GameObject>();

		audioS = GetComponentInParent<AudioSource>();

	}

	
	void OnEnable()
	{
		ReturnToDefaultState();
	}

	
	void LateUpdate()
	{
		if(GameController.Instance.CurrState != GameState.GAME)
			return;
			
		if( warmedUp &&  attemptedToFireThisFrame == false)
		{
			//we have stopped firing this frame
			ReturnToDefaultState();
		}
		attemptedToFireThisFrame = false;
	}

#endregion

	/// <summary>
	/// call in succession every frame from an object wielding weapon
	/// must be called in Update, not LateUpdate
	/// </summary>
	public void AttemptToFire( Vector3 target, string tagToHit)
	{
		
			if(warmedUp == false)
			{
				//update warming up timer
				timeSinceWarmupStarted += Time.deltaTime;
				if(timeSinceWarmupStarted >= warmupTime)
				{
					warmedUp = true;
					// timeSinceWarmupStarted = 0f;
				}
			}
			else
			{
				//did the CD time from last shot elapsed?
				if(readytoFire)
				{
					OnReadyToFire(target, tagToHit);
					audioS.PlayOneShot(fireSound);
					ProjectileStreak++;
					readytoFire = false;

				}
				else
				{
					timeSinceLastShot += Time.deltaTime;
					if(timeSinceLastShot >= timeBetweenShots)
					{
						readytoFire = true;
						timeSinceLastShot = 0f;
					}
				}
			}

			attemptedToFireThisFrame = true;
		
	}

	protected abstract void OnReadyToFire(Vector3 target, string tagToHit);
	
	
	

	protected void ShowFireEffect()
	{
		StopCoroutine(ShowFireEffectCoroutine());
		StartCoroutine(ShowFireEffectCoroutine());
	}

	private IEnumerator ShowFireEffectCoroutine()
	{
		fireEffect.SetActive(true);

		yield return GameController.Instance.Pause(fireEffectDuration);

		fireEffect.SetActive(false);
		
	}
	
	/// <summary>
	/// sets the weapon to the state when you only start to fire it
	/// </summary>
	private void ReturnToDefaultState()
	{
		warmedUp = warmupTime == 0f;
		timeSinceWarmupStarted = 0f;		
		readytoFire = true;
		timeSinceLastShot = 0f;
		ProjectileStreak = 0;

	}

	
}
