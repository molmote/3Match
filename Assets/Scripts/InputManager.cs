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
	private BlockObject selectedBlock;
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
				//if (Input.GetMouseButtonUp(0) || )
				MyLogger.Log("Stopped Dragging");
				// if scored, remove(hide) selected blocks 
				selectedBlock = null;
				isDragging = false;

				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);
				if (hit.collider == null)   // corresponding block is not active
				{
					return;
				}
				var newTile = hit.collider.GetComponent<BlockObject>();
				if (newTile != selectedBlock)
				{
					MyLogger.Log("Detected next block");
					// Do switch animation
					// if there is working match, do next logic
					// if not, move the blocks to their original positions.
				}
			}
			// dragging  
			else
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);
				if (hit.collider == null)   // corresponding block is not active
				{
					return;
				}
				MyLogger.Log("Detected first block");
				var newTile = hit.collider.GetComponent<BlockObject>();
				selectedBlock = newTile;
				selectedPosition = Input.mousePosition;
				// RefreshSelection(newTile);
			}
		}
		else
		{
			// dragging started
			if (Input.GetMouseButtonDown(0))
			{
				MyLogger.Log("Started Dragging");
				isDragging = true;
			}
		}
	}
}
