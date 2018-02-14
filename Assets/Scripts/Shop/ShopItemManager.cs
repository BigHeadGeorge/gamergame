﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItemManager : MonoBehaviour 
{
    // The price of the item
    private int price;

    // The item itself
    private int itemNum;

    // The shop management object
    private ShopManager manager;

    // The game controller script
    private GameController controller;

	// Use this for initialization
	void Start () 
    {
        // Get the shopmanager script off of the object
        manager = GameObject.FindGameObjectWithTag("ShopManager").GetComponent<ShopManager>();
        // Find the gamecontroller object and get its GameController script
        controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        // Give the item a number
        assignNum(gameObject.name);
        Debug.Log(gameObject.name + " : " + itemNum);
	}

    private void FixedUpdate()
    {
        // Keep the price up to date with the shopmanager's price
        price = manager.price;
    }

    // Assign the item a number for use later when giving the player their item
    void assignNum(string name)
    {
        if (name.Contains("Health"))
            itemNum = 1;
        if (name.Contains("MaxHP"))
            itemNum = 2;
        if (name.Contains("Damage"))
            itemNum = 3;
        if (name.Contains("Speed"))
            itemNum = 4;
        if (name.Contains("Bullet"))
            itemNum = 5;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "PlayerBullet")
        {
            // Destroy the bullet
            Destroy(other.gameObject);
            // If the player has enough cash money,
            if (controller.money >= price)
            {
                // Give them the item
                giveItem();
                // Take money from the player 
                controller.money -= price;
                // Destroy the object
                Destroy(gameObject);
                // Increase the price of all the items at the shop
                manager.price *= 2;
            }
            else
                // Tell the player that they don't have enough money
                Debug.Log("Not enough money: " + controller.money + " (" + price + " needed)");
        }
    }

    // Give the player their item
    void giveItem()
    {
        switch(itemNum)
        {
            case 1:
                if(controller.playerMan.Health != controller.playerMan.MaxHealth)
                    controller.playerMan.Health++;
                Debug.Log("heal player");
                break;
            case 2:
                controller.playerMan.MaxHealth++;
                Debug.Log("increase max player health");
                break;
            case 3:
                Debug.Log("increase player damage");
                break;
            case 4:
                controller.playerMan.MaxSpeed += 5f;
                Debug.Log("increase player speed/ max speed/ i dont know yet help me");
                break;
            case 5:
                Debug.Log("increase player bullet size or something");
                break;
        }
    }

}