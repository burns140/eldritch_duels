﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    public Transform parentToReturnTo = null; // Parent placeholder to return to
	public Transform placeholderParent = null; // Parent placeholder

    public GameObject placeholder = null; // Temporary placeholder

    public void OnBeginDrag(PointerEventData eventData) {

        placeholder = new GameObject(); // Create a temporary placeholder
		placeholder.transform.SetParent( this.transform.parent ); // Set parent of temp placeholder to current card's parent
        // Since we set the placeholder's parents equal, we set the hierarchy level to the children's too
		placeholder.transform.SetSiblingIndex( this.transform.GetSiblingIndex() ); 
		parentToReturnTo = this.transform.parent; // Set the parent placeholder to return to  
		placeholderParent = parentToReturnTo; // Set the parent placeholder
		this.transform.SetParent( this.transform.parent.parent ); // Make the parent's parent the card's current parent
		GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData) {

        this.transform.position = eventData.position; // Change card position to wherever you drag it

		if(placeholder.transform.parent != placeholderParent){
			placeholder.transform.SetParent(placeholderParent);
        }   

		int newSiblingIndex = placeholderParent.childCount;

		for(int i=0; i < placeholderParent.childCount; i++) { // Get position of all cards for figuring out current card's position
			if(this.transform.position.x < placeholderParent.GetChild(i).position.x) {

				newSiblingIndex = i;

				if(placeholder.transform.GetSiblingIndex() < newSiblingIndex){
					newSiblingIndex--;
                }

				break;
			}
		}

		placeholder.transform.SetSiblingIndex(newSiblingIndex); // Set the position of card in the parent
    }

    public void OnEndDrag(PointerEventData eventData) {

        this.transform.SetParent( parentToReturnTo ); 
		this.transform.SetSiblingIndex( placeholder.transform.GetSiblingIndex() ); // Set the level to current parent's children
		GetComponent<CanvasGroup>().blocksRaycasts = true;

		Destroy(placeholder); // Destory the temporary placeholder
    }
}
