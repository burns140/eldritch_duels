﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	public DuelScript duelScript; // to access duelscript functions
    public Transform parentToReturnTo = null; // Parent placeholder to return to
	public Transform placeholderParent = null; // Parent placeholder

    public GameObject placeholder = null; // Temporary placeholder

	public Transform startArea = null; // where the card came from

	private int childCount = 0;
	public bool isAttacking = false;

	public bool isBlocking = false;
	public bool battleResolved = false;

	void Start(){
		duelScript = GameObject.Find("DuelScriptObject").GetComponent<DuelScript>();
		this.GetComponent<Button>().enabled = false;
	}
	void Update(){
		if(duelScript.currentPhase == Phase.ATTACK || duelScript.currentPhase == Phase.BLOCK || duelScript.currentPhase == Phase.DISCARD || duelScript.currentPhase == Phase.RECALL){
			this.GetComponent<Button>().enabled = true;
		}else{
			this.GetComponent<Button>().enabled = false;
		}
	}

	
	public void toggleAttack(){
		if(this.transform.parent.name.Equals("HandAreaPanel") && duelScript.currentPhase == Phase.DISCARD){
			duelScript.DiscardCard(this.gameObject);
		}
		else if(this.transform.parent.name.Equals("MyPlayAreaPanel") && duelScript.currentPhase == Phase.ATTACK){
			if(isAttacking){
				isAttacking = false;
				this.GetComponent<Image>().color = Color.white;
				duelScript.RemoveAttacker(this.gameObject);
			}else{
				isAttacking = true;
				this.GetComponent<Image>().color = Color.gray;
				duelScript.AddAttacker(this.gameObject);
			}
		}else if(this.transform.parent.name.Equals("OppPlayAreaPanel") && duelScript.currentPhase == Phase.BLOCK){
			if(!isBlocking && isAttacking){
				duelScript.SetToBlock(this.gameObject);
				this.GetComponent<Image>().color = Color.gray;
				this.isBlocking = true;
			}else if(isAttacking){
				this.GetComponent<Image>().color = Color.red;
				duelScript.RemoveToBlock(this.gameObject);
				this.isBlocking = false;
			}
		}else if(this.transform.parent.name.Equals("MyPlayAreaPanel") && duelScript.currentPhase == Phase.BLOCK){
			if(!isBlocking){
				duelScript.SetBlocker(this.gameObject);
				this.GetComponent<Image>().color = Color.gray;
				this.isBlocking = true;
			}else{
				duelScript.RemoveBlocker(this.gameObject);
				this.GetComponent<Image>().color = Color.white;
				this.isBlocking = false;
			}
		}else if(this.transform.parent.name.Equals("MyPlayAreaPanel") && duelScript.currentPhase == Phase.RECALL){
				duelScript.recallCard(this.gameObject.name);
				duelScript.currentPhase = Phase.MAIN;
				this.gameObject.transform.SetParent(GameObject.Find("HandAreaPanel").transform);
		}
	}

    public void OnBeginDrag(PointerEventData eventData) {
		if(duelScript.currentPhase == Phase.MAIN && duelScript.isMyTurn){
			placeholder = new GameObject(); // Create a temporary placeholder
			startArea = this.transform.parent; // the panel the card starts at
			placeholder.transform.SetParent( this.transform.parent ); // Set parent of temp placeholder to current card's parent
			// Since we set the placeholder's parents equal, we set the hierarchy level to the children's too
			placeholder.transform.SetSiblingIndex( this.transform.GetSiblingIndex() ); 
			parentToReturnTo = this.transform.parent; // Set the parent placeholder to return to  
			placeholderParent = parentToReturnTo; // Set the parent placeholder
			this.transform.SetParent( this.transform.parent.parent ); // Make the parent's parent the card's current parent
			GetComponent<CanvasGroup>().blocksRaycasts = false;
		}
    }

    public void OnDrag(PointerEventData eventData) {
		if(duelScript.currentPhase == Phase.MAIN && duelScript.isMyTurn){
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
    }

    public void OnEndDrag(PointerEventData eventData) {
		if(duelScript.currentPhase == Phase.MAIN && duelScript.isMyTurn){
			//int childCount = this.GetComponent<GridLayoutGroup>().transform.childCount;
			Debug.Log("this:"+this.name);
			Debug.Log("parentToReturnTo:"+parentToReturnTo.name);
			Debug.Log("placeholder:"+placeholder.name);
			Debug.Log("placeholder_siblingindex:"+placeholder.transform.GetSiblingIndex());
			if(childCount <= 7 && !startArea.name.Equals("OppPlayAreaPanel")){
				this.transform.SetParent( parentToReturnTo ); 
				this.transform.SetSiblingIndex( placeholder.transform.GetSiblingIndex() ); // Set the level to current parent's children
				GetComponent<CanvasGroup>().blocksRaycasts = true;
			}
			childCount++;
			Destroy(placeholder); // Destory the temporary placeholder
			
			Debug.Log("Parent: " + startArea.transform.name);
			if(startArea.name.Equals("OppPlayAreaPanel")){
				this.transform.SetParent(startArea.transform);
				parentToReturnTo = null;
				placeholderParent = null;
				startArea = null;
				placeholder = null;
				GetComponent<CanvasGroup>().blocksRaycasts = true;
				return;
			}
			if(startArea.name == "HandAreaPanel" && this.transform.parent.name.Equals("MyPlayAreaPanel") && duelScript.CanCast(this.gameObject)){
				
				Debug.Log("this card:"+this.name);
				duelScript.playMyCard(this.name, this.gameObject);
			}
			else if(startArea.name == "MyPlayAreaPanel" && this.transform.parent.name.Equals("HandAreaPanel")){
				Debug.Log("this card:"+this.name);
				if(duelScript.CanRecall())
					duelScript.recallCard(this.name);
				else
					this.gameObject.transform.SetParent(startArea.transform);
			}else if(startArea.name == "HandAreaPanel" && this.transform.parent.name.Equals("MyPlayAreaPanel") && !duelScript.CanCast(this.gameObject)){
				Debug.Log("Not enough mana");
				this.gameObject.transform.SetParent(startArea.transform);
			}
		}

    }
}
