using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities
{
	public static bool BoundsFullyEnclosed(Vector2 insideBoxMin, Vector2 insideBoxMax, 
	Vector2 enclosingBoxMin, Vector2 enclosingBoxMax)
	{
		
		return insideBoxMin.x > enclosingBoxMin.x &&
			insideBoxMin.y > enclosingBoxMin.y &&
			insideBoxMax.x < enclosingBoxMax.x &&
			insideBoxMax.y < enclosingBoxMax.y;
		
	}
}
