using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DifficultyToWave
{
	public EnemyType[] typeArr;
	public GameObject[] spawnCheckpoints;
}

public class SpawnController : Singleton<SpawnController> {

	[SerializeField] GameObject[] enemyPrefabs;
	[SerializeField] DifficultyToWave[] difficultyToWaveArr;
	[SerializeField] float spawnPause;
	[SerializeField] float spawnCoolDown;
	int difficulty;
	//helpers
[SerializeField]	float timeSinceLastSpawn;
	

#region Unity funcs
	
	protected override void Awake()
	{
		base.Awake();
	}

	
	void Update()
	{
		if(GameController.Instance.CurrState != GameState.GAME)
			return;

			
		timeSinceLastSpawn += Time.deltaTime;
		if(timeSinceLastSpawn >= spawnCoolDown)
		{
			SpawnNewWave();
			timeSinceLastSpawn = 0f;
		}
	}


#endregion

#region spawner funcs
	public void AdvanceDifficultyLevel()
	{
		difficulty ++;
	}

	void SpawnNewWave()
	{
		Vector3 spawnPos = GetRandomSpawnCheckpoint();
		EnemyType[] typeArr= difficultyToWaveArr[difficulty].typeArr;

		StartCoroutine(SpawnWaveCoroutine(spawnPos, typeArr));
	}

	IEnumerator SpawnWaveCoroutine(Vector3 spawnCheckpoint, EnemyType[] typeArr)
	{
		float xDelta = difficulty >= 3 ? 10f : 30f;
		float yDelta = difficulty <= 3 ? 10f : 30f;
		
		for (int i = 0; i < typeArr.Length; i++)
		{

			Vector3 actualSpawnPos = GetRandomSpawnPos(spawnCheckpoint, xDelta, yDelta);

			SpawnEnemy(GetPrefabOfEnemyType(typeArr[i]), actualSpawnPos);
			yield return GameController.Instance.Pause(spawnPause);
		}
	}

	void SpawnEnemy(GameObject enemy, Vector3 spawnPos)
	{
		Instantiate(enemy, spawnPos, Quaternion.identity);
	}
#endregion

#region helpers
	Vector3 GetRandomSpawnCheckpoint()
	{
		GameObject[] spawnPosArr = difficultyToWaveArr[difficulty].spawnCheckpoints;

		int randIndex = Random.Range(0, spawnPosArr.Length );
		return spawnPosArr[randIndex].transform.position;
	}

	Vector3 GetRandomSpawnPos(Vector3 spawnCheckpoint, float maxDeltaX, float maxDeltaY)
	{
		float x = Random.Range(0f, maxDeltaX);
		float y = Random.Range(0f, maxDeltaY);

		spawnCheckpoint.x += x;
		spawnCheckpoint.y += y;

		return spawnCheckpoint;
	}

	GameObject GetPrefabOfEnemyType(EnemyType type)
	{
		switch(type)
		{
			case EnemyType.Scout:
				return enemyPrefabs[0];

			case EnemyType.RifleEnemy:
				return enemyPrefabs[1];

			default:
				return null;
		}
	}
#endregion
}

[System.Serializable]
public enum EnemyType 
{
	Scout, RifleEnemy
}
