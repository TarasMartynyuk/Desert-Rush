using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Projectile : MonoBehaviour 
{

	[SerializeField] GameObject splashEfect;
	[SerializeField] GameObject projectileSprite;
	
	[SerializeField]  int damage ;
	public Collider2D shooterCollider;
	public float Speed;
	bool collidedWithTarget ;
	//write-only :D
	public string TagToHit { protected get;  set;}
	GameObject hitTarget;
	Vector2 hitPoint;
	
	
	
	void Awake()
	{
		Assert.IsNotNull(projectileSprite);
		Assert.IsNotNull(splashEfect);
		
	}
	
	Vector2 target;

	
	public void LaunchLinear(GameObject projectile, Vector3 target, string tagToHit, 
		Collider2D shooterCol, float splashEffectDuration)
	{
		StartCoroutine(LaunchLinearCoroutine(projectile, target, tagToHit, shooterCol, splashEffectDuration));
	}
	
	

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other == shooterCollider)
		{
			// print("touched father collider");
			return;
		}

		//fly throuhg items and projectiles, and enemies(if not targeting them)		
		if(other.tag == "Projectile" || other.tag =="Item" || (other.tag == "Enemy" && TagToHit != "Enemy"))
		{
			// print("hit projectile");
			return;
		}

		if(other.tag == "Shield" && TagToHit == "Enemy")
		{
			return;
		}
		
		collidedWithTarget = true;
		hitTarget = other.gameObject;
		// hitPoint = other.contacts[0].point;
		hitPoint = transform.position;
		
		
	}

	///shows splash effect, applies damage to target
	/// then destroys projectile 
	void OnTargetHit(float explosionDuration)
	{
		if(TagToHit == null)
		{
			throw new System.ArgumentNullException("Projectile script attached to " + 
				gameObject.name + "does not have TagToHit configured");
		}

		if(TagToHit == hitTarget.tag)
		{
				
				Actor actorScript = hitTarget.GetComponent<Actor>();
				actorScript.SufferDamage(damage);
				//do damage
			
		}
		

		//
		StartCoroutine(Explode(explosionDuration));
		
	}

	IEnumerator LaunchLinearCoroutine(GameObject projectile, Vector3 target, string tagToHit, 
		Collider2D shooterCol, float splashEffectDuration)
	{
		
		//tell projectile whom to damage
		TagToHit = tagToHit;
		//tell her whom to ignore
		shooterCollider = shooterCol;

		Vector3 direction = target - projectile.transform.position;
		direction.z = 0f;
		direction.Normalize();

		while(true)
		{

			//check to see if enemy was hit(enemy of the Enemy is Player :D)
			if(collidedWithTarget)
			{
				OnTargetHit(splashEffectDuration);
				break;
			}
			else
			{
				//if not, move forward
				projectile.transform.position += direction * Speed * Time.deltaTime;
			}

			yield return GameController.Instance.WaitForEndOfFrameSync();
		}

		
	}

	IEnumerator Explode( float explosionDuration )
	{
		projectileSprite.SetActive(false);
		splashEfect.SetActive(true);
		splashEfect.transform.position = new Vector3(hitPoint.x, hitPoint.y, 0f);
		

		yield return GameController.Instance.Pause(explosionDuration);

		splashEfect.SetActive(false);
		Destroy(gameObject);
	}
	
	
}
