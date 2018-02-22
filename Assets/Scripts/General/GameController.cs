﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * IDEA FOR COMPLETE GAME REDESIGN: 
 * 
 * Instead of WAVE-BASED COMBAT, give the player a FUEL BAR
 * SHOOTING makes FUEL go DOWN
 * Killing ENEMIES makes them EXPLODE into FUEL, HEALTH, MONEY
 * 
 * The TWIST; the FUEL BAR is actually more like a SPEED BAR
 * 
 * MORE FUEL makes you GO FASTER
 * 
 * THE GAME CAN BE LEVEL BASED INSTEAD, WHERE THE PLAYER IS 
 * ENCOURAGED TO GET A TON OF FUEL AND SPEED TO THE END OF THE LEVEL
 * 
 * -POWER UP THAT MAKES YOU LOSE NO FUEL
 * -Enemy level goes up the further into the level you get
 **/

// TODO: Make bosses for the end of each level

// TODO: REBALANCE LITERALLY EVERYTHING BEFORE YOU EVEN CONSIDER THE GAME AS DONE (but don't do this until you've finished every other todo and the game itself)

// FIXME: Player / enemy boosters rendering behind asteroids and portal particles

// TODO: Spawn more enemies per wave, maybe spawn them in bursts
// TODO: Make enemy spawn rate based on the player's speed
// TODO: Change the asteroids in the second level so that they're randomly picked on their own rather than being a part of the normal enemy pool
    // This way, the enemy level can just be set back to 1 at the start of level 2 instead of the weird shit that's going on right now

/// <summary>
/// Handles levels, spawning enemies, creating the shop entrance,
/// player score and money, and UI elements
/// </summary>
public class GameController : MonoBehaviour
{
    // The camera's screen shake component
    public ScreenShaker shaker;

    // UI Elements
    public Text[] UIElements = new Text[5];

    // Enemies to spawn
    public GameObject[] enemies = new GameObject[6];

    // Bunch of variables
    private int level = 1;
    public int enemyLevel = 2;
    // The player's score earned from destroying enemies and stuff
    public int score = 0;
    // The player's money earned from destroying enemies that they can spend
    public int money = 0;
	private int wave = 1;
	private int spawns = 5;
	public float spawnRate = 1.5f;
	public float waveRate = 7f;
	public int enemyWeapon;

    // Should the game wait before starting the wave???
    public bool waitBeforeWave = true;

    // Is it the start of a level?
    public bool levelStart = true;

    //Enemies to spawn
	public GameObject enemy;
	public GameObject mine;

	// Spawn point for enemies
	//private Vector2 enemySpawnPos;

    // Player Stuff
    private GameObject player;
    public PlayerManager playerMan;

    // The shop
    public GameObject shopEntrance;

    // The asteroid field in level 2
    public GameObject asteroidField;

    // The player's bullet prefab
    public GameObject playerBullet;

    // The boss that's currently alive and some other stuff about it
    public GameObject boss;
    public BossManager bossMan;
    public int bossNum;
    public bool bossIsAlive = false;

    private void Start()
    {
		// Get the player object so the script can snatch some stuff off the player
		// IDK why this reference is here anymore, I think some other scripts
		// leech off of this one for player info
        player = GameObject.FindGameObjectWithTag("Player");
        playerMan = player.GetComponent<PlayerManager>();

        // Set the player's bullet size back to normal when the game starts
        playerBullet.transform.localScale = new Vector3(0.440893f, .3f, 0.440893f);

        // Start spawning enemies
        StartCoroutine(spawnEnemies());
        //StartCoroutine(level1BossBattle());
    }

    // Update is called once per frame
    void Update () 
	{
        // Update text
		updateText();
    }

    /// <summary>
    /// Update any UI text elements that show stuff like speed or score
    /// </summary>
    void updateText()
    {
        // Get the player's speed value, then "truncate" by taking substring
        string speedMsg = playerMan.Speed.ToString();
        int startIndex = speedMsg.IndexOf(".");
        UIElements[3].text = speedMsg.Substring(0, startIndex + 2);
    }

    /// <summary>
    /// Spawns a given amount of enemies
    /// </summary>
    public IEnumerator spawnEnemies()
    {
        // If the enemy level is high enough for the controller to not spawn enemies,
        if (enemyLevel == 5 && level == 1)
            // Move to the next level
            level2Init();
        // If true,
        if (levelStart)
        {
            // Set up the level text
            setLevelText();
            // Show the leveltext element
            UIElements[4].gameObject.SetActive(true);
            // Wait for a few seconds
            yield return new WaitForSeconds(3);
            // Hide the text
            UIElements[4].gameObject.SetActive(false);
        }
        // Make sure we don't wait again next time around
        //levelStart = false;

        // Disable the wait before wave cus yeah
        waitBeforeWave = false;

        // Spawn <spawns> enemies
        for(int enemies = 0; enemies <= spawns; enemies++)
        {
            // Create an enemy
            enemyCreation();
            // Wait to spawn another enemy
            yield return new WaitForSeconds(spawnRate);
        }

        // Set up next wave
        StartCoroutine(nextWaveSetup());
        // Exit the method
        yield break;
    }

    /// <summary>
    /// Sets the game up for the next wave by giving the player a chance to
    /// enter the shop, increasing the spawn rate, increasing the total enemies
    /// in each wave, and sets the next wave up to display the level text.
    /// </summary>
    public IEnumerator nextWaveSetup()
    {
        // Increase the range of enemies allowed
        enemyLevel++;

        //int shopSpawn = (int)Random.Range(1, 4);
        //if (shopSpawn == 3)
        // Spawn the shop entrance
        Instantiate(shopEntrance, new Vector3(8, 0, 0), transform.rotation);

        // Wait to start the next wave
        yield return new WaitForSeconds(waveRate);

        // Transition into the next wave
        // While the spawnRate is above a certain num,
        if (spawnRate >= .5f)
            // Decrease it
            spawnRate -= .1f;
        // Double the spawns
        spawns *= 2;
        // Start a new sublevel
        levelStart = true;
        // Go back to spawning enemies
        StartCoroutine(spawnEnemies());
        // Exit the method
        yield break;
    }

    /// <summary>
    /// Called by the Shop Entrance, keeps the player from losing speed
    /// and waits until the coroutine is stopped.
    /// </summary>
    public IEnumerator shopWait()
    {
        // Start waiting
        waitBeforeWave = true;
        // Constantly wait while the method is running
        while (true)
            yield return new WaitForSeconds(1);
    }

    /// <summary>
    /// Sets up for creating things in the second level.
    /// </summary>
    void level2Init()
    {
        // Change the level to 2
        level++;
        // Enable the asteroid field
        asteroidField.SetActive(true);
        //Increase maxLevel???
        //Add asteroid enemies to pool
    }

    /// <summary>
    /// Picks an enemy to spawn and creates it at a random position
    /// </summary>
    void enemyCreation()
    {
        // Generate a random spawn position for the enemy
        Vector2 spawnPos = new Vector2(9.5f, Random.Range(-4f, 4f));
        // Generate a random weapon for the enemy based on how high the current level is
        enemyWeapon = Random.Range(1, enemyLevel);

        // Spawn an enemy off the list of enemies based on the weapon that was picked
        Instantiate(enemies[enemyWeapon - 1], spawnPos, transform.rotation);
    }

    /// <summary>
    /// Sets up the level text to display the correct level.
    /// </summary>
    void setLevelText()
    {
        int levelOffset = 1;
        // Set up the offset to make the level appear properly
        if (level == 2)
            levelOffset = 4;
        // Set up the level text element to show the right stuff
        UIElements[4].text = "Level " + level + " - " + (enemyLevel - levelOffset);
    }

    IEnumerator level1BossBattle()
    {
        // Update the status of the boss
        bossIsAlive = true;
        // Get the boss
        boss = GameObject.FindGameObjectWithTag("Level1Boss");
        bossMan = boss.GetComponent<BossManager>();
        // wait while the boss is alive
        while(bossIsAlive)
            yield return new WaitForSeconds(1f);
        //give the player some powerups to pick from
        yield break;
    }

    /// <summary>
    /// Pause the game for a tiny fraction of a second when called.
    /// </summary>
    private IEnumerator FreezeGame()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(.03f);
        Time.timeScale = 1f;
        yield break;
    }

    /// <summary>
    /// Freeze the game for a tiny fraction of a second when called.
    /// </summary>
    public void Freeze()
    {
        StartCoroutine(FreezeGame());
    }

    /** FIRST BOSS BATTLE **
     *- Has a tiny gun that fires in 3-bullet bursts, normal bullets
     *- Pulls back and fires out a bunch of projectiles that can
     *  blow up into a bunch of health and speed and stuff
     *- Blows up into a bunch of health and stuff for the player upon defeat
     *- Has "arms" on the top and bottom of the screen that shoot vertically moving missiles
     */

}