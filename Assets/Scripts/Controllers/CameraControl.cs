using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// moves camera alongside Bill
/// </summary>
public class CameraControl : MonoBehaviour {

	new Camera camera;
	[SerializeField] RectTransform mapBounds;
	
	
	//helper vars
	Vector3 newCameraPos;
	Vector3[] corners;


	
	
	void Awake () 
	{
		camera = Camera.main;

		//leave the camera Z coordinate untouched
		newCameraPos.z = camera.transform.position.z;

		corners = new Vector3[4];
		mapBounds.GetWorldCorners(corners);
		
	}
	
	void Update () 
	{	
		if(GameController.Instance.CurrState != GameState.GAME)
			return;
			
		//change only x and y since we are 2D
		
		if(gameObject.transform.position.x >= corners[0].x + camera.orthographicSize * camera.aspect &&
			gameObject.transform.position.x <= corners[2].x - camera.orthographicSize * camera.aspect)
		{
			newCameraPos.x = gameObject.transform.position.x;
		}

		if(gameObject.transform.position.y >= corners[0].y + camera.orthographicSize &&
			gameObject.transform.position.y <= corners[2].y - camera.orthographicSize)
		{
			newCameraPos.y = gameObject.transform.position.y;		
		}

		camera.transform.position = newCameraPos;
		
		
	}

	
}
