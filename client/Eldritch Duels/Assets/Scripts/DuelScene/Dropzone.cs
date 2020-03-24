using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Dropzone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData) {

        if(eventData.pointerDrag == null){
			return;
        }

		Draggable d = eventData.pointerDrag.GetComponent<Draggable>(); // Get draggable from current card
		if(d != null) {
			d.placeholderParent = this.transform;
		}
    }

    public void OnPointerExit(PointerEventData eventData) {

        if(eventData.pointerDrag == null)
			return;

		Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
		if(d != null && d.placeholderParent==this.transform) {
			d.placeholderParent = d.parentToReturnTo; // Set parent when you drag out of previous parent
		}
    }

    public void OnDrop(PointerEventData eventData) {
        Debug.Log (eventData.pointerDrag.name + " was dropped on " + gameObject.name);

		Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
		if(d != null) {
			d.parentToReturnTo = this.transform; // Set current parent after drag done
		}
    }
}
