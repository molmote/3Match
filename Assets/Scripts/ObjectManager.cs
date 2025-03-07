using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    List<BlockObject> blockObjectPool;

    void OnEnable()
    {
		blockObjectPool = new List<BlockObject>();
	}
}
