using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    Dictionary<(int, int), PlaceHolder> blockHolders = new Dictionary<(int, int), PlaceHolder>();

    // Start is called before the first frame update
    void Start()
    {
        var holders = GetComponentsInChildren<PlaceHolder>();

        foreach (var holder in holders) 
        {
            if (blockHolders.ContainsKey((holder.Col, holder.Row)))
            {
				Debug.Log($"{holder.Col}, {holder.Row} = duplicated");
			}
            blockHolders[(holder.Col, holder.Row)] = holder;

            var block = ObjectManager.Instance.GetBlock();

            //Debug.Log($"{holder.Col}, {holder.Row} = ");
		}
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
