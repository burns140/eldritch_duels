using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DropzoneAI : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData) {

        if(eventData.pointerDrag == null){
			return;
        }

		DraggableAI d = eventData.pointerDrag.GetComponent<DraggableAI>(); // Get draggable from current card
		if(d != null) {
			d.placeholderParent = this.transform;
		}
    }

    public void OnPointerExit(PointerEventData eventData) {

        if(eventData.pointerDrag == null)
			return;

		DraggableAI d = eventData.pointerDrag.GetComponent<DraggableAI>();
		if(d != null && d.placeholderParent==this.transform) {
			d.placeholderParent = d.parentToReturnTo; // Set parent when you drag out of previous parent
		}
    }

    public void OnDrop(PointerEventData eventData) {

		int childCount = this.GetComponent<GridLayoutGroup>().transform.childCount;
		//Debug.Log("first child:"+this.GetComponent<GridLayoutGroup>().transform.GetChild(0).name);

		// Test code to check how to get column number of target card
		for(int i=0; i<childCount; i++){
			if(this.GetComponent<GridLayoutGroup>().transform.GetChild(i).name == "Test 0"){
				//Debug.Log("i:"+i);
			}
		}

        //Debug.Log (eventData.pointerDrag.name + " was dropped on " + gameObject.name);

		DraggableAI d = eventData.pointerDrag.GetComponent<DraggableAI>();
		if(d != null && childCount < 7) {
			d.parentToReturnTo = this.transform; // Set current parent after drag done
			//Debug.Log(StartCoroutine(CoWaitForPosition(d)));
		}
    }

	IEnumerator CoWaitForPosition(DraggableAI d)
	{
		yield return new WaitForEndOfFrame();
		// Find position of objects in grid
		Debug.Log("Draggable:" + d.transform.position.x);
	}
}
