using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class EnemyParameterStatus : BaseParameterStatus {
	public int spiritNum = 1;			//when death, how many Spirit produces.
	public int friendPossibility = 25;	//possibility to become friend. (0 - 100)

	public Spawner spawner; //if it is not null this spawns from this.

}
