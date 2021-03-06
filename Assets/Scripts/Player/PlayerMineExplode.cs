﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Make explosion damage scale with the player's damage

public class PlayerMineExplode : MonoBehaviour
{
    // Store the screen shake script on the camera
    private ScreenShaker shaker;

    private void Start()
    {
        // Snatch the main camera's screen shake script
        shaker = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<ScreenShaker>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If the explosion collided with an enemy,
        if (collision.gameObject.tag == "Enemy")
        {
            // If the enemy is a normal enemy,
            if (collision.gameObject.name.Contains("Enemy"))
            {
                // Get it's management script
                EnemyManager2 enemyMan = collision.gameObject.GetComponent<EnemyManager2>();
                // Set the health to 0, maybe tweak this later so it takes health based on player damage?
                enemyMan.Health = 0;
            }
            // If the enemy is a mine or an asteroid or something,
            else
            {
                // Get the management script
                EnemyManager1 enemyMan = collision.gameObject.GetComponent<EnemyManager1>();
                // Update it's health, it doesn't need health but it's a property so changing the value makes it do a specific thing
                enemyMan.Health--;
            }
        }

        // If the explosion collided with the player,
        if(collision.gameObject.name == "Player")
        {
            // Snatch the player's management script
            PlayerManager playerMan = collision.gameObject.GetComponent<PlayerManager>();
            // Set their health to 0 after doing a double-check to make sure they're alive
            if(collision.gameObject != null)
                playerMan.Health = 0;
        }

        // Apply screen shake
        shaker.ShakeCamera(.4f);
    }

}
