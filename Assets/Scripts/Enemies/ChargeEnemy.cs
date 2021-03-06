﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeEnemy : Enemy
{
    private float timer = 0f;
    private Rigidbody rb;
    private Vector3 sourcePos;
    private bool charging;
    private Tile currentTile;

    private int[] myTile = new int[2];

    new public static event EnemyDeath OnEnemyDeath;
    public float timeToCharge;
    public int damage;
    public int speed;

    private bool enRoute = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        sourcePos = rb.position;
        charging = false;
        timeToCharge = Random.Range(4.0f, 6.0f);
        audio = GetComponent<UnitAudio>();
        //bm = board.GetComponent<BoardManager>();
        myTile[0] = pos.x;
        myTile[1] = pos.y;
        //boardSize = bm.tiles.Length;
        health += health * difficultyMultiplier * (bm.GetLevel() - 1);
        maxHealth = health;
        //Dumb way to get the health UI to update
        LoseHealth(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            Die();
        }
        timer += Time.deltaTime;
        if (timer >= timeToCharge && !charging && !enRoute)
        {
           if (Random.Range(-10, 10) >= -2)
           {
               Charge();
           }
           else
           {
                //DISABLE IS ISSUES ARISE
               setMovement();
               Move(myTile);
           }
           timer = 0f;
           timeToCharge = Random.Range(4.0f, 6.0f);
        }
        else if (rb.position.z < -20)
        {
            rb.position = new Vector3(rb.position.x, rb.position.y, 20);
            enRoute = true;
            charging = false;
        }
        else if (Vector3.Distance(sourcePos, rb.position) < .5 && !charging)
        {
            rb.velocity = Vector3.zero;
            rb.position = sourcePos;
            enRoute = false;
        }
    }

    private void Charge()
    {
        charging = true;
        rb.velocity = new Vector3(0, 0, -1 * speed);
    }

    // Before deleting gameObject, make call to enemy manager to decrement enemy count
    public override void Die()
    {
        base.Die();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.GetComponent<Player>().LoseHealth(damage);
        } else if (other.gameObject.CompareTag("Tile") && charging)
        {
            other.gameObject.GetComponent<Tile>().WarnTile();
            currentTile = other.gameObject.GetComponent<Tile>();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Tile"))
        {
            other.gameObject.GetComponent<Tile>().UnwarnTile();
        }
    }

    private void setMovement()
    {
        switch (Random.Range(1, 5))
        {
            case 1: //move down
                if (bm.checkUpGridAvailibility(myTile[0], myTile[1], pos.x + 1, myTile[1]))
                {
                    myTile[0] = pos.x + 1;
                }
                break;
            case 2: //move up
                if (bm.checkUpGridAvailibility(myTile[0], myTile[1], pos.x - 1, myTile[1]))
                {
                    myTile[0] = pos.x - 1;
                }
                break;
            case 3: //move side
                if (bm.checkUpGridAvailibility(myTile[0], myTile[1], myTile[0], pos.y + 1))
                {
                    myTile[1] = pos.y + 1;
                }
                break;
            case 4: //move up
                if (bm.checkUpGridAvailibility(myTile[0], myTile[1], myTile[0], pos.y - 1))
                {
                    myTile[1] = pos.y - 1;
                }
                break;
        }

        if (myTile[0] > 2 || myTile[0] < 0) myTile[0] = 1;
        if (myTile[1] > 8) myTile[1] = 7;
        if (myTile[1] < 5) myTile[1] = 6;
    }
}
