﻿using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Enemy : MovingObject
{    
    private float maxHealth;
    private float currentHealth;
    private float maxMana;
    private float currentMana;
    private int range;
    private string state;
    private Animator animator;
    private readonly string[] states = new string[] { "Attacking", "Charging", "Blocking", "Healing" };
    private static Enemy instance;
    private bool firstUpdate = true; 

    // Start is called before the first frame update
    protected override void Start()
    {        
        maxHealth = 5;
        currentHealth = maxHealth;
        maxMana = 5;
        currentMana = 0;
        state = null;
        animator = GameObject.Find("EnemyGraphics").GetComponent<Animator>();
        range = 3;        
        base.Start();
               
    }

    private void Update()
    {        
        if (SceneManager.GetActiveScene().name == "Overworld")
        {
            Vector3 playerPos = Player.GetInstance().transform.position;
            float x1 = transform.position.x - range;
            float x2 = transform.position.x + range;
            float y1 = transform.position.y - range;
            float y2 = transform.position.y + range;

            if (playerPos.x >= x1 && playerPos.x <= x2 && playerPos.y >= y1 && playerPos.y <= y2)
            {
                //Debug.Log("Player in range");

                int yDir = 0;
                int xDir = 0;
                if (playerPos.x > transform.position.x)
                {
                    xDir = 1;
                }
                else if (playerPos.x < transform.position.x)
                {
                    xDir = -1;
                }

                if (playerPos.y > transform.position.y)
                {
                    yDir = 1;
                }
                else if (playerPos.y < transform.position.y)
                {
                    yDir = -1;
                }

                if (IsMoveable())
                {                    
                    Move(xDir, yDir);
                }
            }
        }
        else if(SceneManager.GetActiveScene().name == "Battle")
        {
            if (firstUpdate)            //bandaid solution??
            {
                transform.position = new Vector3(1.5f, 4.5f, 0);
                UpdateHealthBar();
                UpdateManaBar();
                firstUpdate = false;
            }
        }
    }

    public void ChooseMove()
    {
        int move = Random.Range(0, states.Length);              
        state = states[move];
        animator.SetBool(state, true);       
    }

    public void EnactMove()
    {
        //create enums for states
        if (state == "Attacking" && currentMana > 0)
        {           
            currentMana--;
            UpdateManaBar();
            Player.GetInstance().TakeDamage(1);
        }
        else if (state == "Charging" && currentMana < maxMana)
        {
            currentMana++;
            UpdateManaBar();
        }
        else if (state == "Healing" && currentMana > 0 && currentHealth < maxHealth)
        {
            currentMana--;
            currentHealth++;
            UpdateManaBar();
            UpdateHealthBar();
        }
        animator.SetBool("Charging", false);
        animator.SetBool("Healing", false);
        animator.SetBool("Attacking", false);
        animator.SetBool("Blocking", false);
    }

    private void UpdateManaBar()
    {
        GameObject.Find("EnemyManaNumbers").GetComponent<TextMeshPro>().SetText(currentMana.ToString());
        GameObject.Find("EnemyManaBar").GetComponent<HealthBar>().SetSize(currentMana / maxMana);
    }

    private void UpdateHealthBar()
    {
        GameObject.Find("EnemyHealthNumbers").GetComponent<TextMeshPro>().SetText(currentHealth.ToString());
        GameObject.Find("EnemyHealthBar").GetComponent<HealthBar>().SetSize(currentHealth / maxHealth);
    }

    public void ResetState()
    {
        state = null;
    }

    public void TakeDamage(int dmg)
    {
        if (state != "Blocking")
        {                 
            currentHealth -= dmg;
            UpdateHealthBar();
            animator.SetBool("Damaged", true);

            if (currentHealth == 0)
            {
                //player wins
                GameManager.GetInstance().SetWorldState("GruntDead", true);                
                Loader.Load(SceneName.Overworld, Player.GetInstance().OverworldPosition());
            }
        }
    }

    public void PrepareForBattle()
    {
        DisableMovement(true);
        SetDestination(new Vector3(1.5f, 4.5f, 0));
        DontDestroyOnLoad(gameObject);
    }

    public void OnDestroy()
    {
        Debug.Log("GG");
    }

}