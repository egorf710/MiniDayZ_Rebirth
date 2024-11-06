using Assets._scripts.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.UI;

public enum Effect { Bleeding, Regeneration, SickProtection, Sick, Adrinaline, Energizer }

public class PlayerCharacteristics : MonoBehaviour, Vulnerable
{
    [SerializeField] private Slider healthField;
    [SerializeField] private Slider waterField;
    [SerializeField] private Slider foodField;
    [SerializeField] private Slider heatField;
    [Space]
    [SerializeField] private Image healthImage;
    [SerializeField] private Sprite[] healthSprite;
    [SerializeField] private Image bleedingImage;
    [SerializeField] private Image speedUpImage;
    [SerializeField] private Image sickImage;
    [SerializeField] private GameObject bleedingParticlePrefab;
    [Space]

    public List<effectCase> effectCases = new List<effectCase>();
    [Serializable]
    public struct effectCase
    {
        public Effect effectType;
        public int effectTime;
    }

    /// <summary>
    /// —корость траты воды и еды и т.д  
    /// speedSpending food and water and other
    /// </summary>
    [SerializeField] private float speedSpending;
    [SerializeField] private float logTime;
    private float spendTimer, logTimer;

    private int currentHealth;
    private int currentWater;
    private int currentFood;
    private int currentHeat;

    public int playerHealth
    {
        get { return currentHealth; }
        set { currentHealth = value; Refresh(); if (value <= 0) { RestartPlayer(); } }
    }
    public int playerWater
    {
        get { return currentWater; }
        set { currentWater = value; Refresh(); }
    }
    public int playerFood
    {
        get { return currentFood; }
        set { currentFood = value; Refresh(); }
    }
    public int playerHeat
    {
        get { return currentHeat; }
        set { currentHeat = value; Refresh(); }
    }

    [Space]

    public int handDamage = 15;
    public int armor = 0;
    public int blockChanse = 10;
    public int handCritChanse = 10;

    private bool IsLocalPlayer()
    {
        return GetComponent<PlayerNetwork>().isLocalPlayer;
    }
    public void InitUI
        
        (Slider healthField, Slider waterSlider, Slider foodField, Slider heatField,
        Image healthImage, Image bleedingImage, Image speedUpImage, Image sickImage
        )
    {
        if (!IsLocalPlayer()) { enabled = false; return; }
        this.healthField = healthField;
        this.waterField = waterSlider;
        this.foodField = foodField;
        this.heatField = heatField;

        this.speedUpImage = speedUpImage;
        this.sickImage = sickImage;
        this.healthImage = healthImage;
        this.bleedingImage = bleedingImage;
    }

    private void Refresh()
    {
        IsLocalPlayer();
        //UI
        currentHealth = Mathf.Clamp(currentHealth, 0, 100);
        currentWater = Mathf.Clamp(currentWater, 0, 100);
        currentFood = Mathf.Clamp(currentFood, 0, 100);
        currentHeat = Mathf.Clamp(currentHeat, 0, 100);

        healthField.value =  currentHealth;
        waterField.value = currentWater;
        foodField.value = currentFood;
        heatField.value = currentHeat;
    }

    private IEnumerator IEUpdate()
    {
        while(true)
        {
            int playerHealthRegen = 0;
            if (Time.time >= logTimer)
            {
                if (currentWater <= 35 && currentWater > 0)
                {
                    Debug.Log("я хочу выпить чего-нибудь");
                }
                if (currentFood <= 35 && currentFood > 0)
                {
                    Debug.Log("я хочу съесть что-то");
                }
                if (currentFood <= 0)
                {
                    Debug.Log("я умираю от голода");
                }
                if (currentWater <= 0)
                {
                    Debug.Log("я умираю от жажды");
                }
                logTimer = Time.time + logTime;
            }
            if(currentWater >= 50 && currentFood >= 50 && currentHeat >= 50)
            {
                playerHealthRegen = 1;
            }
            if(Time.time >= spendTimer)
            {
                playerFood--;
                playerWater -= 2;
                spendTimer = Time.time + speedSpending;
            }
            if (effectCases.Count > 0)
            {
                List<effectCase> effectsToRemove = new List<effectCase>();

                bool stopRegeneration = false;

                foreach (var effect in effectCases)
                {
                    if (effect.effectType == Effect.Bleeding)
                    {
                        if (effect.effectTime >= Time.time)
                        {
                            playerHealth--;
                            playerHealthRegen = 0;
                            stopRegeneration = true;
                        }
                        else
                        {
                            bleedingImage.gameObject.SetActive(false);
                            effectsToRemove.Add(effect);
                            stopRegeneration = false;
                        }
                    }
                    else if (effect.effectType == Effect.Regeneration && effect.effectTime >= Time.time && !stopRegeneration)
                    {
                        playerHealthRegen++;
                    }
                    else if (effect.effectType == Effect.Adrinaline)
                    {
                        if (effect.effectTime >= Time.time)
                        {
                            playerHealthRegen = 50;
                        }
                        else
                        {
                            effectsToRemove.Add(effect);
                            speedUpImage.gameObject.SetActive(false);
                            PlayerMove playerMove = InventoryManager.Instance.player.GetComponent<PlayerMove>();
                            playerMove.base_movement_speed = 3.5f;
                        }
                    }
                    else if (effect.effectType == Effect.Energizer && effect.effectTime < Time.time)
                    {
                        effectsToRemove.Add(effect);
                        speedUpImage.gameObject.SetActive(false);
                        PlayerMove playerMove = InventoryManager.Instance.player.GetComponent<PlayerMove>();
                        playerMove.base_movement_speed = 3.5f;
                    }
                    else if (effect.effectType == Effect.Sick)
                    {
                        if (effect.effectTime >= Time.time)
                        {
                            playerHealthRegen = 0;
                        }
                        else
                        {
                            sickImage.gameObject.SetActive(false);
                        }
                    }
                }
                foreach (var effect in effectsToRemove)
                {
                    effectCases.Remove(effect);
                }
            }
            {
                playerHealth += playerHealthRegen;
            }
            {
                healthImage.sprite = healthSprite[Mathf.Clamp(playerHealthRegen, 0, 2)];
            }
            yield return new WaitForSeconds(1f);
        }
    }
    private void Start()
    {
        if(!GetComponent<PlayerNetwork>().isLocalPlayer) { return; }
        playerHealth = 100;
        playerFood = 100;
        playerWater = 100;
        playerHeat = 100;
        StartCoroutine(IEUpdate());
        //Time.timeScale = 4f;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetEffect(Effect.Bleeding, 32);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetEffect(Effect.Regeneration, 40);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetEffect(Effect.Energizer, 40);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SetEffect(Effect.Adrinaline, 40);
        }
    }
    public void SetEffect(Effect effectType, int time = 60)
    {
        if(effectType == Effect.Bleeding)
        {
            bleedingImage.gameObject.SetActive(true);
            ParticleSystem particleSystem = Instantiate(bleedingParticlePrefab, transform).GetComponent<ParticleSystem>();
            var main = particleSystem.main;
            main.duration = time / 10;
            particleSystem.Play();
            Destroy(particleSystem.gameObject, time + 5);
        }
        else if (effectType == Effect.Energizer)
        {
            speedUpImage.gameObject.SetActive(true);
            PlayerMove playerMove = InventoryManager.Instance.player.GetComponent<PlayerMove>();
            playerMove.base_movement_speed += (playerMove.base_movement_speed / 100 * 10);
        }
        else if (effectType == Effect.Adrinaline)
        {
            speedUpImage.gameObject.SetActive(true);
            PlayerMove playerMove = InventoryManager.Instance.player.GetComponent<PlayerMove>();
            playerMove.base_movement_speed += (playerMove.base_movement_speed / 100 * 10);
        }
        else if (effectType == Effect.Sick)
        {
            sickImage.gameObject.SetActive(true);
        }
        effectCases.Add(new effectCase()
        {
            effectType = effectType,
            effectTime = (int)(Time.time + time)
        });
    }

    public void TakeDamage(int damage)
    {
        if(armor < damage)
        {
            if(UnityEngine.Random.Range(0, 100) <= blockChanse)
            {
                //block
                DebuMessager.Mess("blocked", Color.gray);
                return;
            }
            playerHealth -= (damage - armor);
        }
        InventoryManager.ClothesDamage(damage <= 10 ? 1 : 2);
    }
    public void RestartPlayer()
    {
        InventoryManager.DropAllInventory();
        effectCases.Clear();
        playerHealth = 100;
        playerFood = 100;
        playerWater = 100;
        playerHeat = 100;

        ServerManager.TeleportToSpawn();
    }
}
