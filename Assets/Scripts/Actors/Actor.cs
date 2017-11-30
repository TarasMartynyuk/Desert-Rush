using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Actor : MonoBehaviour {

	[SerializeField] protected float speed = 1f;
	[SerializeField] protected int health;

	public abstract void SufferDamage(int damage);
}
