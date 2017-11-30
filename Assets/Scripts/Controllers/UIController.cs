using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : Singleton<UIController> 
{	
	//state ui change support
	[SerializeField] GameObject [] UIs;
	
	
	//ITEM ADD support
	[SerializeField] GameObject itemPanel;
	[SerializeField] GameObject genericItemIcon;
	[SerializeField] Text endgameText;
	[SerializeField] Text endgameButtonText;
	
	Dictionary<Item, GameObject> itemToIconDict;

	//tooltip support
	[SerializeField] Canvas canvas;
	[SerializeField]  GameObject GameUI;
	
	[SerializeField]  GameObject tipPrefab;
	[SerializeField]  Player pl;
	
	
	Text tipText;
	
	
	Image tipImage;
	
	public float tipStayTime;
	
	bool mainTipTurn;
	
	


	
	protected override void Awake()
	{
		base.Awake();
		itemToIconDict = new Dictionary<Item, GameObject>();

	}

#region State Ui change

	public void ShowUIWithTag(string tagToShow, bool haveWon = false, bool playerDead = false)
	{
		

		foreach (var item in UIs)
		{
			item.SetActive(item.tag == tagToShow);
		}

		if(tagToShow == "Endgame")
		{
			if(haveWon)
			{
				endgameText.text = "You reach " + 
					"your base safely and tell the command where to find the raiders, they'll be dead in no time!";
				endgameButtonText.text = "Yay!";
			}
			else
			{
				endgameText.text = playerDead ? "Your dead body slowly sinks into vast sand " +
				"while raiders loot what they can from what's left of your buggy..." : "Without the vehicle " + 
				"and supplies you die slowly in the desert. You'd better get shot...";
				endgameButtonText.text = haveWon ? "Yay!" : "Damn.";
			}
			
		}
	}

#endregion

	public void SpawnTipAtPos(Vector3 worldPosition, string tipString, float scale = 1f)
	{
		

		//first you need the RectTransform component of your canvas
 		RectTransform CanvasRect = canvas.GetComponent<RectTransform>();

		//then you calculate the position of the UI element
 		//0,0 for the canvas is at the center of the screen, 
		//  whereas WorldToViewPortPoint treats the lower left corner as 0,0. 
		//  Because of this, you need to subtract the height / 
		// width of the canvas * 0.5 to get the correct position.

		Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(worldPosition);

		Vector2 WorldObject_ScreenPosition=new Vector2(
 			((ViewportPosition.x*CanvasRect.sizeDelta.x)-(CanvasRect.sizeDelta.x*0.5f)),
 			((ViewportPosition.y*CanvasRect.sizeDelta.y)-(CanvasRect.sizeDelta.y*0.5f)));

		//now you can set the position of the ui element
		

		StartCoroutine(ShowTip(tipStayTime,  tipString, scale, WorldObject_ScreenPosition));

		mainTipTurn = !mainTipTurn;

	}

	IEnumerator ShowTip(float fadeTime, string tipString, float scale, Vector2 WorldObject_ScreenPosition)
	{
		GameObject tipGo = Instantiate(tipPrefab, GameUI.transform);
		tipGo.transform.localScale = new Vector3(scale, scale, scale);

		RectTransform tip = tipGo.GetComponent<RectTransform>();

		Text textToChange = tipGo.GetComponentInChildren<Text>();
		textToChange.text = tipString;

 		tip.anchoredPosition = WorldObject_ScreenPosition;

		yield return GameController.Instance.Pause(tipStayTime);

		yield return StartCoroutine(FadeTip(tipGo, 2f));

		Destroy(tipGo);
	}

	IEnumerator FadeTip(GameObject tipGO, float fadeTime)
	{
		Image tipImage = tipGO.GetComponent<Image>();
		if(tipImage == null)
		{
			Debug.LogError("tip Image is null! - UIController.FadeTip");
		}

		tipImage.CrossFadeAlpha(0f, fadeTime, false);
		yield return GameController.Instance.Pause(fadeTime);
		Destroy(tipGO);
	}


#region items
	public void AddIconToInventory(Item item, Sprite itemSprite)
	{
		//add a generic, image-less icon to item panel
		GameObject icon = Instantiate(genericItemIcon, itemPanel.transform);
		Image iconImage = icon.GetComponent<Image>();

		iconImage.sprite = itemSprite;

		//save entry
		itemToIconDict.Add(item, icon);
	}

	public void RemoveItemIconFromInvemntory(Item item)
	{
		GameObject icon = itemToIconDict[item];

		if(icon == null)
		{
			Debug.LogError("Trying to delete item icon that is not in the dictionary");
			return;
		}

		Destroy(icon);
	}
#endregion
}
