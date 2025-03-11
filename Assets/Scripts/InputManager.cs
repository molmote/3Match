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
	private bool isMoving = false;
	private PlaceHolder selectedBlock;
	Vector3 selectedPosition;
	private float time = 0;

	// [SerializeField] GameData gameData; // uses scriptable object for customized settings

	void Update()
    {
		time += Time.deltaTime;
		// UpdateTime(time);

		if (Board.Instance.IsMoving()) // prevent interruption
		{
			return;
		}

		if (isDragging)
		{
			// dragging finished
			//float movedDistance = Vector3.Distance (Input.mousePosition, selectedPosition);
			//MyLogger.Log($"{movedDistance} > {thresholdMove}");
			//if (movedDistance > thresholdMove)
			{
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

					// do check swap

					Board.Instance.SwapBlock(newTile, selectedBlock);
					bool con1 = Board.Instance.CheckMatch4(newTile);
					bool con2 = Board.Instance.CheckMatch4(selectedBlock);
					if (con1 || con2)
					{ 
						//if(con1) Board.Instance.CheckMatch4(newTile);
						//if(con2) Board.Instance.CheckMatch4(selectedBlock);
						///Board.Instance.SwapBlock(newTile, selectedBlock);
						MyLogger.Log("CheckMatch4");
					}
					else if (Board.Instance.CheckMatch3(newTile) || Board.Instance.CheckMatch3(selectedBlock))
					{
						MyLogger.Log("CheckMatch3 CheckMatch3");
						//Board.Instance.CheckMatch4(newTile);
						//Board.Instance.CheckMatch4(selectedBlock);
						///Board.Instance.SwapBlock(newTile, selectedBlock);
					}
					else
					{
						int j = 0;
						Board.Instance.SwapBlock(newTile, selectedBlock);
						// do moving and revert animation
					}
					
					selectedBlock = null;
					isDragging = false;
					isMoving = true; // clear after one second
					MyLogger.Log("Stopped Dragging");

					Board.Instance.ClearBlock();
				}
			}
			// dragging  
			//else
			{
			//	isDragging = false;
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
