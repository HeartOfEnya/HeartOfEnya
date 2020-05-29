﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A replacement for the old ingredient button that will (hopefully) support drag-&-drop
/// </summary>
public class DraggableIngredient : MonoBehaviour
{
	public Ingredient ingredient; //ingredient we're using
	public Vector3 startPos; //where we originally spawned, and where to return to if not dropped in the pot
	public Pot pot;
	public Vector3 infoCanvasOffset; //offset for the UI canvas

	private bool inSoup = false; //are we in da soup?
	private bool dragging = false; //whether we're currently being dragged
	private BoxCollider2D potCollider; //stores a reference to the pot's collider component for quick access. Set automatically in Start()
	private BoxCollider2D myCollider; //stores a reference to our collider component for quick access. Set automatically in Start()
	private GameObject hoverUI; //stores a reference to the group for the hover-over UI for quick access. Set automatically in Start()
	private Canvas myCanvas; //stores a reference to the child canvas object for quick access. Set automatically in Start()

    // Start is called before the first frame update
    void Start()
    {
    	//set up some references we'll need later
    	hoverUI = transform.Find("HoverUI").gameObject; //store a reference for later use so we don't have a fuckton of Find() calls
        myCanvas = transform.Find("HoverUI").transform.Find("Canvas").GetComponent<Canvas>(); //store this bastard, we're gonna use it a lot
        
    	//update text & images from ingredient
        UpdateData();

        //drag-&-drop stuff
        startPos = transform.position; //store our initial position
        //get some quick references to the pot & ingredient colliders
        potCollider = pot.GetComponent<BoxCollider2D>();
        if(!potCollider)
        {
        	Debug.LogError("ERROR! Pot has no collider!");
        }
        myCollider = GetComponent<BoxCollider2D>();
        if(!myCollider)
        {
        	Debug.LogError("ERROR! Ingredient has no collider!");
        }

        //turn off the UI on startup so it only appears when moused over
        hoverUI.SetActive(false);

        //For reasons no sane person can explain, the only way to get the fucking UI to show up is if we switch the UI Canvas
        //from "WorldSpace" render mode to "SceenSpace - Camera" and then switch it back.
        //This is probably the single dumbest thing I have ever done in my life, and that counts all the times I intentionally electrocuted myself
        //just to see if my braces were conductive.
        myCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        myCanvas.renderMode = RenderMode.WorldSpace;
        //adjust it's fucking coords because who the fuck cares what I entered in the inspector, toss it out at the first opportunity
        //and make me do it again. Keep the original Z value though - it's magic and I don't want to poke it lest I invite the wrath of god
        myCanvas.transform.localPosition = new Vector3(infoCanvasOffset.x, infoCanvasOffset.y, myCanvas.transform.localPosition.z);
    }

    // Update is called once per frame
    void Update()
    {
    	//handle drag-&-drop
        if (dragging)
        {
        	//because everything's so fucking small I've gotta turn these screenspace coords into worldspace
        	//lest the ingredient ping off into space like a goddamn rocket the second you click it
        	Vector3 point = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));
            transform.position = new Vector3(point.x, point.y, 0.0f); //make real fucking sure the zero Z value is preserved because I can't trust anyone anymore
        }
    }

    //if clicked, start dragging
    private void OnMouseDown()
    {
    	//when clicked, start dragging the object
    	dragging = true;
    	//if we're in the soup, remove ourselves
    	if(inSoup)
    	{
    		pot.RemoveIngredient(this); //get out of the pot
    		SoupManager.main.DisableIngredient(ingredient); //get out of the soup manager
    		inSoup = false;
    	}
    }

    //if unclicked, stop dragging
    private void OnMouseUp()
    {
    	//when unclicked, stop dragging the object
    	dragging = false;
    	
    	//check if we were dropped in the soup & respond accordingly
    	if (myCollider.bounds.Intersects(potCollider.bounds))
    	{
    		//we were dropped in the soup - do we fit?
    		if (pot.AddIngredient(this))
    		{
    			//b e c o m e   s o u p
    			inSoup = true; //only have to update inSoup - AddIngredient took care of our position for us
    			bool added = SoupManager.main.EnableIngredient(ingredient); //tell the soup manager to add us
    			//Error checking just on the offchance that somehow the ingredient could be added to the pot but not the soup manager.
    			//That should be impossible, but better safe than sorry. 
    			if (added) //success
    			{
    				Debug.Log("Ingredient " + ingredient.name + " successfully added to soup!");
    			}
    			else //failure
    			{
    				Debug.LogWarning("Ingredient " + ingredient.name + " can fit in soup, but wasn't added properly!");
    			}
    		}
    		else
    		{
    			//no more room
    			Debug.Log("Oh no! Pot is full");
    			transform.position = startPos; //go back home and cry because the popular kids rejected us yet again
    		}
    	}
    	else
    	{
    		//we were not dropped in the soup
    		transform.position = startPos; //go back home and cry because you missed the pot
    	}
    }

    //detect mouse over & display the target/effect data
    private void OnMouseEnter()
    {
    	hoverUI.SetActive(true); //turn on UI
    }

    //detect mouse no longer over & hide the target/effect data
    private void OnMouseExit()
    {
    	hoverUI.SetActive(false); //turn off UI
    }

    /// <summary>
    /// Updates images & text in the event that the ingredient changes.
    /// </summary>
    public void UpdateData()
    {
    	/***ADD THE REST OF THIS ONCE THE UI IS ADDED***/

    	//if ingredient isn't set, give an error message & abort
    	// if(!ingredient)
    	// {
    	// 	Debug.Log("ERROR: No ingredient specified for button " + myID);
    	// 	return;
    	// }

    	//

    	//set name text based on ingredient
        // Text nameTxt = transform.Find("NameText").GetComponent<Text>();
        // nameTxt.text = ingredient.name;
        //set effect text based on ingredient
        // Text effectTxt = transform.Find("EffectText").GetComponent<Text>();
        Text effectTxt = myCanvas.transform.Find("EffectText").GetComponent<Text>();
        effectTxt.text = ingredient.GetEffectText();
        //set ingredient icon based on ingredient
        SpriteRenderer ingIcon = GetComponent<SpriteRenderer>();
        ingIcon.sprite = ingredient.ingredientIcon;
    	//set character icon based on ingredient
        // Image charIcon = transform.Find("CharacterIcon").GetComponent<Image>();
        Image charIcon = myCanvas.transform.Find("CharacterIcon").GetComponent<Image>();
        charIcon.sprite = ingredient.characterIcon;
    }
}
