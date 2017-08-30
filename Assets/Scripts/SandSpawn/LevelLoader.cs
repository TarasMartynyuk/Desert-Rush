using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class ColorToPrefab {
	public bool onTop;
	public Color32 color;
	public GameObject[] prefabArr;
}

public class LevelLoader : MonoBehaviour {

	public string levelFileName;

	//public Texture2D levelMap;

	public ColorToPrefab[] colorToPrefab;


	// Use this for initialization
	void Awake () 
	{
		LoadMap();
	}

	void EmptyMap() {
		// Find all of our children and...eliminate them.

		while(transform.childCount > 0) {
			Transform c = transform.GetChild(0);
			c.SetParent(null); // become Batman
			Destroy(c.gameObject); // become The Joker
		}
	}

	void LoadAllLevelNames() {
		// Read the list of files from StreamingAssets/Levels/*.png
		// The player will progess through the levels alphabetically
	}

	void LoadMap() {
		EmptyMap();

		// Read the image data from the file in StreamingAssets
		string filePath = Application.dataPath + "/StreamingAssets/" + levelFileName;
		byte[] bytes = System.IO.File.ReadAllBytes(filePath);
		Texture2D levelMap = new Texture2D(2, 2);
		levelMap.LoadImage(bytes);


		// Get the raw pixels from the level imagemap
		Color32[] allPixels = levelMap.GetPixels32();
		int width = levelMap.width;
		int height = levelMap.height;

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {

				SpawnTileAt( allPixels[(y * width) + x], x, y );

			}
		}
	}

	void SpawnTileAt( Color32 c, int x, int y ) {

		// If this is a transparent pixel, then it's meant to just be empty.
		if(c.a <= 0) {
			return;
		}

		// Find the right color in our map

		// NOTE: This isn't optimized. You should have a dictionary lookup for max speed
		foreach(ColorToPrefab ctp in colorToPrefab) {
			
			if( c.Equals(ctp.color) ) 
			{
				if(ctp.onTop)
				{
					//firs place sand
					GameObject sand = (GameObject)Instantiate(GetRandomPrefab(colorToPrefab[0].prefabArr), 
						2 * new Vector3(x, y, 0), Quaternion.identity );
					sand.transform.SetParent(this.transform);
				}
				
				// Spawn the prefab at the right location
				GameObject randomPrefab = GetRandomPrefab(ctp.prefabArr);
				
				GameObject go = (GameObject)Instantiate(randomPrefab, 2 * new Vector3(x, y, 0), Quaternion.identity );
				go.transform.SetParent(this.transform);
				// maybe do more stuff to the gameobject here?
				return;
			}
		}

		// If we got to this point, it means we did not find a matching color in our array.

		Debug.LogError("No color to prefab found for: " + c.ToString() );

	}

    private GameObject GetRandomPrefab(GameObject[] prefabArr)
    {
		if(prefabArr.Length == 1)
		{
			return prefabArr[0];
		}
		
		int index = UnityEngine.Random.Range(0, prefabArr.Length);
		return prefabArr[index];
    }
}
