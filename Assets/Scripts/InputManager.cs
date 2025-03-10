using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

	}

	[SerializeField] float thresholdMove;

	private bool isDragging = false;
	private PlaceHolder selectedBlock;
	Vector3 selectedPosition;
	private float time = 0;

	// [SerializeField] GameData gameData; // uses scriptable object for customized settings

	// Update is called once per frame
	void Update()
    {
		time += Time.deltaTime;
		// UpdateTime(time);

		if (isDragging)
		{
			// dragging finished
			float movedDistance = Vector3.Distance (Input.mousePosition, selectedPosition);
			if (movedDistance > thresholdMove)
			{
				// MyLogger.Log($"{movedDistance} > {thresholdMove}");
				//if (Input.GetMouseButtonUp(0) || )
				// if scored, remove(hide) selected blocks 

				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);
				if (hit.collider == null)   // corresponding block is not active
				{
					return;
				}
				var newTile = hit.collider.GetComponent<PlaceHolder>();
				if (newTile != selectedBlock)
				{
					MyLogger.Log($"Detected next block {newTile.Block.name} ({newTile.Col},{newTile.Row})");
					// Do switch animation
					// if there is working match, do next logic
					// if not, move the blocks to their original positions.

					Board.Instance.SwapBlock(newTile, selectedBlock);
					selectedBlock = null;
					isDragging = false;
					MyLogger.Log("Stopped Dragging");
				}
			}
			// dragging  
			else
			{
			}
		}
		else
		{
			// dragging started
			if (Input.GetMouseButtonDown(0))
			{
				MyLogger.Log("Started Dragging");

				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);
				if (hit.collider == null)   // corresponding block is not active
				{
					return;
				}
				var newTile = hit.collider.GetComponent<PlaceHolder>();
				MyLogger.Log($"Detected first block {newTile.Block.name} ({newTile.Col},{newTile.Row})");
				selectedBlock = newTile;// PlaceHolder.Block;
				selectedPosition = Input.mousePosition;
				// RefreshSelection(newTile);

				isDragging = true;
			}
		}
	}
}
