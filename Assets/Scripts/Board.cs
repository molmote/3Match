using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BlockObject;

public class Board : MonoBehaviour
{
    Dictionary<(int, int), PlaceHolder> blockHolders = new Dictionary<(int, int), PlaceHolder>();
	//[SerializeField] List<int[]> defaultColors = new List<int[]>();
	[SerializeField] List<PlaceHolderRow> defaultSetup;

	public static Board Instance
	{ get; private set; }

    void Awake()
    {
        Instance = this;
    }

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

            int x = holder.Col + 3;

			var block = ObjectManager.Instance.GetBlock(defaultSetup[x].GetColor(holder.Row));
            holder.SetBlock(block); 
            block.gameObject.SetActive(true);

            Debug.Log($"{holder.Col}, {holder.Row} = {defaultSetup[x].GetColor(holder.Row)}");
		}
	}

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsMatchAfterSwap(BlockObject a, BlockObject b)
    {
        return false;
    }

    public bool SwapBlock(PlaceHolder a, PlaceHolder b)
    {
        if (a != b)
        {
            BlockObject tmp = a.Block;
            a.SetBlock(b.Block);
            b.SetBlock(tmp);
		}
        return true;
    }
}
