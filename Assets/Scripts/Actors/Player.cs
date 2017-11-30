using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Actor 
{

	[SerializeField] RectTransform mapBounds;
	[SerializeField] int vehicleInteractionRange = 16;
	[SerializeField] Image healthbar;
	[SerializeField] Image weaponIcon;	
	[SerializeField] Vehicle vehicle;	
	
	
	
	//all weapon prefabs
	[SerializeField] List<GameObject> weaponGOs;
	
	//item support
	List<Item> itemsOnHand;
	Weapon weapon;
	int currWeaponIndex = 0;

	//movement support
	Rigidbody2D rgb;
	float horizMovementAmount, vertMovementAmount;
	Vector2 displacementThisFrame;
	Vector2 newPos;
	
	Vector3 mousePos;
	

	//movement clamp support
	SpriteRenderer spr;
	Vector2 spriteMin;
	Vector2 spriteMax;
	Vector2 boundsMin, boundsMax;

	//UI
	float maxHealth;
	public bool tipsShown;
	
#region Unity methods
	void Awake()
	{
		spr = GetComponent<SpriteRenderer>();
		if(spr == null)
		{
			// Debug.LogError("no spriterenderer on player");
		}

		rgb = GetComponent<Rigidbody2D>();
		if(rgb == null)
		{
			// Debug.LogError("no rigidbody2D on player");
		}

		itemsOnHand = new List<Item>(1);

		maxHealth = health;
		healthbar.fillAmount = 1f;

		//make the first weapon in the list selected
		EquipWeaponAtIndex(0);

		Vector3[] corners = new Vector3[4];
		mapBounds.GetWorldCorners(corners);

		boundsMin = corners[0];
		boundsMax = corners[2];


		

	}

	void Update()
	{
		if(GameController.Instance.CurrState != GameState.GAME)
			return;

			
		//check for movement
		//rotate to face cursor
		mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		transform.rotation = Quaternion.LookRotation(Vector3.forward, mousePos - transform.position);

		//update weapon firing status
		if(Input.GetAxisRaw("Fire1") == 1)
			weapon.AttemptToFire( mousePos, "Enemy");


		//interact with vehicle
		if(Input.GetKeyDown(KeyCode.E))
		{
			InteractWithVehicle();
		}

		//shield
		if(Input.GetKeyDown(KeyCode.R))
		{
			TryActivateShield();
		}


		//change equiped weapon
		if(Input.GetKeyDown(KeyCode.F))
			EquipNextWeapon();


		//gather movement info
		displacementThisFrame.x = Input.GetAxis("Horizontal");
		displacementThisFrame.y = Input.GetAxis("Vertical");

		if(displacementThisFrame == Vector2.zero)
			return;
		
		displacementThisFrame.Normalize();
		displacementThisFrame *= speed * Time.deltaTime;

		//estimate the botLeft and topRight points of players's sprite next frame if moved by *displacementThisFrame*
		spriteMin.x = spr.bounds.min.x + displacementThisFrame.x;
		spriteMin.y = spr.bounds.min.y + displacementThisFrame.y;

		spriteMax.x = spr.bounds.max.x + displacementThisFrame.x;
		spriteMax.y = spr.bounds.max.y + displacementThisFrame.y;

	}

	void FixedUpdate()
	{
		if(GameController.Instance.CurrState != GameState.GAME)
			return;

			
		newPos = rgb.position;

		//check if the movement wont get the player out of map bounds
		if(spriteMin.x > boundsMin.x && spriteMax.x < boundsMax.x)
		{
			//move horizontally
			newPos.x += displacementThisFrame.x;

		}

		if(spriteMin.y > boundsMin.y && spriteMax.y < boundsMax.y)
		{
			//move vertically
			newPos.y += displacementThisFrame.y;

		}

		if(newPos != rgb.position)
			rgb.MovePosition(newPos);
		
	}

	
	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == "Item")
		{
			

			Item itemScript = other.GetComponent<Item>();
			if(itemScript == null)
			{
				Debug.LogError("No item script obj on collider with tag Item!");
			}
			itemScript.OnPickup(this);

			// foreach (var item in itemsOnHand)
			// {
			// 	print(item);
			// }
		}
	}
#endregion

	public void AddWeapon(GameObject weaponPrefab)
	{
		weaponGOs.Add(weaponPrefab);
	}

	public void AddItem(Item item)
	{
		itemsOnHand.Add(item);
	}

	public override void SufferDamage(int damage)
	{
		health -= damage;
		healthbar.fillAmount = (float) health / maxHealth;

		if(health <= 0)
		{
			
			// GameController.Instance.SpawnBloodCloud(transform.position);
			GameController.Instance.TransferToEndGameState(false, true);

		}
	}

	public void ShowStartTips()
	{
		// if(tipsShown)
		// 	return;

		Vector3 tipOffset = Vector3.right *  16f + Vector3.up * 4f;
		
		StartCoroutine(ShowTipCoroutine(1f, tipOffset,  
			"You are returning to base after a mission " +
			"when you stumble upon raiders' outpost, freshly constructed in location. You don't have enough supplies to survive journey back, " +
			"so your best chance is to move past as quicly as possible"));

		tipOffset = Vector3.left *  15f + Vector3.down * 4f;
		
		StartCoroutine(ShowTipCoroutine(1f + UIController.Instance.tipStayTime / 1.3f, tipOffset, 
			"Unfortunately, you have just run out of fuel!" +
			"It can be found near the road on both sides, if your lucky. If not" +
			" - venture to the north, chance of finding it there is higher"));

		tipOffset = Vector3.right *  12f ;

		StartCoroutine(ShowTipCoroutine(4f * UIController.Instance.tipStayTime, tipOffset, 
			"Your buggy is equipped with bubble shield! " +
			"come near it and press 'R' to activate it "));
		
		StartCoroutine(ShowTipCoroutine(6.7f * UIController.Instance.tipStayTime, tipOffset,
			"Tip : the best time to go away from buggy is just after you have repelled raiders' attack!"));
		



	}

	// public void ShowShieldTips()
	// {
	// 	if(tipsShown)
	// 		return;

	// 	StartCoroutine(ShowShieldTipCoroutine(30f));
	// }

#region helpers

	void InteractWithVehicle()
	{
		if(Vector3.Distance(transform.position, GameController.Instance.Vehicle.transform.position) <=
			 vehicleInteractionRange)
				{
					if(itemsOnHand.Count != 0)
					{
						
						
						
							itemsOnHand[itemsOnHand.Count - 1].OnVehicleInteraction(vehicle);
							itemsOnHand.RemoveAt(itemsOnHand.Count - 1);
						
						
					}
					
				}
	}

	void TryActivateShield()
	{
		if(Vector3.Distance(transform.position, GameController.Instance.Vehicle.transform.position) <=
			 vehicleInteractionRange)
		{
			vehicle.TryActivateShield();
		}
	}

	void EquipNextWeapon()
	{
		if(weaponGOs.Count == 1)
			return;

		//hide curr weapon
		weaponGOs[currWeaponIndex].SetActive(false);

		currWeaponIndex++;
		if(currWeaponIndex >= weaponGOs.Count)
			currWeaponIndex = 0;

		EquipWeaponAtIndex(currWeaponIndex);
		
		
	}

	void EquipWeaponAtIndex(int index)
	{
		weaponGOs[index].SetActive(true);
		weapon = weaponGOs[index].GetComponent<Weapon>();
		weaponIcon.sprite = weapon.sprite;
	}

	IEnumerator ShowTipCoroutine(float timeToWait, Vector3 tipOffset, string message)
	{
		yield return GameController.Instance.Pause(timeToWait);


		UIController.Instance.SpawnTipAtPos(transform.position + tipOffset , message);
		
	}

	IEnumerator ShowShieldTipCoroutine(float timeToWait)
	{
		yield return GameController.Instance.Pause(timeToWait);

		UIController.Instance.SpawnTipAtPos(transform.position + Vector3.right *  15f + Vector3.up * 4f, 
		"Your buggy is equipped with bubble shield! " +
		"come near it and press 'R' to activate it ");
	}
#endregion
}
