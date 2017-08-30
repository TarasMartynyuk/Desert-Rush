using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shield : MonoBehaviour {

	[SerializeField] GameObject shieldGO;
	[SerializeField] Image shieldRechargeBar;
	[SerializeField] float shieldDuration;
	
	[SerializeField] float shieldCD;
	float currCharge;

	
	void Update()
	{
		if(currCharge != shieldCD)
		{
			currCharge += Time.deltaTime;
			if(currCharge > shieldCD)
				currCharge = shieldCD;

			updateShieldBar();
		}
	}

	public bool TryActivate()
	{
		if(currCharge != shieldCD)
			return false;

		StartCoroutine(ActivateShield(shieldDuration));
		currCharge = 0f;
		updateShieldBar();

		return true;
	}

	void updateShieldBar()
	{
		shieldRechargeBar.fillAmount = currCharge / shieldCD;
	}

	IEnumerator ActivateShield(float time)
	{
		shieldGO.SetActive(true);

		yield return GameController.Instance.Pause(time);

		shieldGO.SetActive(false);
	}

}
