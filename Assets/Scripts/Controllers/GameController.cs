using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : Singleton<GameController> {

	[SerializeField] Texture2D cursor;
	[SerializeField] GameObject bloodCloudPrefab;
	
	//references for enemy
	[SerializeField] GameObject player;
	[SerializeField] GameObject vehicle;
	[SerializeField] GameObject shield;
	
	[SerializeField] BackgroundMusicPlayer musicPlayer;
	

	public GameObject Player{get {return player;}}	
	public GameObject Vehicle{get {return vehicle;}}
	public GameObject Shield{get {return shield;}}
	
	public GameState CurrState{get; protected set;} 

#region Unity funcs
	
	protected override void Awake()
	{
		base.Awake();
		
		CurrState = GameState.MENU;
		
		UIController.Instance.ShowUIWithTag("Menu");
	}

	

	
	void Update()
	{
		if(Input.GetAxisRaw("Cancel") == 1)
			TransferToPauseState();
	}

#endregion

#region Game State control
	

	public void TransferToMenuState()
    {
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        CurrState = GameState.MENU;

		UIController.Instance.ShowUIWithTag("Menu");
		
    }
    
	public void TransferToPauseState()
    {
        CurrState = GameState.PAUSE;
		UIController.Instance.ShowUIWithTag("Pause");
		musicPlayer.Pause();

		Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

	public void ResumeGame()
	{
		CurrState = GameState.GAME;

		UIController.Instance.ShowUIWithTag("Game");

		musicPlayer.Resume();

		Cursor.SetCursor(cursor, new Vector2(10, 10), CursorMode.Auto);
	}

    public void StartGame()
	{
		ResumeGame();

		Player pl = player.GetComponent<Player>();
		if(pl == null)
		{
			Debug.LogError("pl == null ");
		}

		pl.ShowStartTips();
		
	}

	public void TransferToEndGameState(bool haveWon, bool playerDead)
	{
		CurrState = GameState.ENDGAME;
		UIController.Instance.ShowUIWithTag("Endgame", haveWon, playerDead);

		Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
	}

	public void Quit()
	{
		Application.Quit();
	}

#endregion


#region pause support
	public  Coroutine Pause(float seconds)
	{
        return StartCoroutine(PauseRoutine(seconds)); 
    }

	public Coroutine WaitForEndOfFrameSync()
	{
		return StartCoroutine(WaitForEndOfFrameRoutine());
	}

	/// <summary>
	/// pauses itself while GameState is Pause
	/// then waits for *seconds* amount of sec
	/// </summary>
	/// <param name="seconds"></param>
	/// <returns></returns>
	private IEnumerator PauseRoutine(float seconds)
	{
        while (CurrState == GameState.PAUSE) 
		{
            yield return new WaitForFixedUpdate(); 
        }

        yield return new WaitForSeconds(seconds);
	}

	/// <summary>
	/// pauses itself while GameState is Pause
	/// then waits for *seconds* amount of sec
	/// </summary>
	/// <param name="seconds"></param>
	/// <returns></returns>
	private IEnumerator WaitForEndOfFrameRoutine()
	{
        while (CurrState == GameState.PAUSE) 
		{
            yield return new WaitForFixedUpdate(); 
        }

        yield return new WaitForEndOfFrame();
	}


#endregion
	public void SpawnBloodCloud(Vector3 position)
	{
		StartCoroutine(SpawnBloodCloudCoroutine(position));
	}

	IEnumerator SpawnBloodCloudCoroutine(Vector3 position)
	{
		
		Quaternion rotation = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0, 4) * 90f);
		GameObject bloodCloud = Instantiate(bloodCloudPrefab, position, Quaternion.identity);

		yield return Pause(1.5f);

		Destroy(bloodCloud);
	}
}

public enum GameState
{
	MENU, GAME, PAUSE, ENDGAME
}
