using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.UIElements;


public enum AnimationControllerState { none, rifle, pistol}
public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private string path_to_items = "/AnimationItems/";

    [Header("Compenets")]
    [SerializeField] private SpriteRenderer bodyskin;
    [SerializeField] private SpriteRenderer head;
    [SerializeField] private SpriteRenderer body;
    [SerializeField] private SpriteRenderer pants;
    [SerializeField] private SpriteRenderer backpack;
    [SerializeField] private SpriteRenderer vest;
    [SerializeField] private SpriteRenderer hand;


    [Header("AnimationVars")]
    public AnimationControllerState controllerState;
    public float AnimationSpeed;
    public Vector2 animationDir;
    public float speed;
    public bool shoot;
    public bool interact;

    [Header("AnimationData")]
    public string cur_bodyskin;
    public string cur_head;
    public string cur_body;
    public string cur_pants;
    public string cur_backpack;
    public string cur_hand;
    public string cur_vest;

    [Header("DEBUG")]
    public string itemName;
    public AnimationThpe animationType;
    public bool SET;
    public bool CLEAR;
    public bool SHOOT;
    public bool SHOOTING;
    public Vector2 ShootDir;
    public float timeInteract;
    public bool INTERACT;

    [Header("Network")]
    public PlayerNetwork playerNetwork;
    [SerializeField] private int repeate = 5;

    //private void Update()
    //{
    //    if (SET)
    //    {
    //        SET = false;
    //        SetClothByName(itemName);
    //    }
    //    if (CLEAR)
    //    {
    //        CLEAR = false;
    //        if (itemName == "")
    //        {
    //            ClearClothByType(animationType);
    //        }
    //        else
    //        {
    //            ClearClothByName(itemName);
    //        }
    //    }
    //    if (SHOOT)
    //    {
    //        SHOOT = false;
    //        Shoot(ShootDir);
    //    }
    //    if (SHOOTING)
    //    {
    //        Shoot(ShootDir);
    //    }
    //    if (INTERACT)
    //    {
    //        INTERACT = false;
    //        Interact(timeInteract);
    //    }
    //}
    private void Start()
    {
        SetClothByName("default_skin");
        StartCoroutine(Animating());
    }
    public void AnimMove(Vector2 dir)
    {
        dir.Normalize();
        if (dir.magnitude > 0)
        {
            animationDir = dir;
        }
        speed = dir.magnitude;
    }
    
    private IEnumerator Animating()
    {
        int index = 0;
        int rep = 0;
        while (true)
        {
            if (index > 3) { index = 0; }
            if (!interact)
            {
                if (speed > 0 && !shoot)
                {
                    AnimMove(ref index);
                }
                else
                {
                    AnimIdle(ref index);
                    shoot = false;
                }

            }
            if(rep >= repeate)
            {
                if (playerNetwork.isLocalPlayer)
                {
                    playerNetwork.CMDAnimSync(interact, shoot, speed, animationDir);
                }
                rep = 0;
            }
            rep++;

            while (INTERACT)
            {
                if (index > 3) { index = 0; }
                AnimInteract(ref index);
                yield return new WaitForSeconds(1 / AnimationSpeed);
            }

            yield return new WaitForSeconds(1 / AnimationSpeed);
        }
    }
    private IEnumerator IEInteract(float time = 1)
    {
        interact = true;

        StartCoroutine(IEInteractAnimating());

        yield return new WaitForSeconds(time);

        playerNetwork.GetComponent<PlayerMove>().enabled = true;
        playerNetwork.GetComponent<WeaponController>().itsEnable = true;

        interact = false;
    } 
    private IEnumerator IEInteractAnimating()
    {
        int index = 0;
        while (interact)
        {
            if (index > 3) { index = 0; }
            AnimInteract(ref index);
            yield return new WaitForSeconds(1 / AnimationSpeed);
        }
    }

    private void AnimInteract(ref int index)
    {
        bodyskin.sprite = bodyskin_interact[index];
        if (head_interact.Length > 0)
        {
            head.sprite = head_interact[index];
        }
        if (bodyskin_interact.Length > 0)
        {
            body.sprite = body_interact[index];
        }
        if (pants_interact.Length > 0)
        {
            pants.sprite = pants_interact[index];
        }
        if (backpack_interact.Length > 0)
        {
            backpack.sprite = backpack_interact[index];
        }
        if (vest_interact.Length > 0)
        {
            vest.sprite = vest_interact[index];
        }
        if (hand_interact.Length > 0)
        {
            hand.sprite = hand_interact[index];
        }
        index++;
    }

    private void AnimMove(ref int index)
    {
        int side = GetSideByDir(animationDir);
        if (side == 0)
        {
            bodyskin.sprite = bodyskin_run_down[index];
            if (head_run_down.Length > 0)
            {
                head.sprite = head_run_down[index];
            }
            if (bodyskin_run_down.Length > 0)
            {
                body.sprite = body_run_down[index];
            }
            if (pants_run_down.Length > 0)
            {
                pants.sprite = pants_run_down[index];
            }
            if (backpack_run_down.Length > 0)
            {
                backpack.sprite = backpack_run_down[index];
            }
            if (vest_run_down.Length > 0)
            {
                vest.sprite = vest_run_down[index];
            }
            if (hand_run_down.Length > 0)
            {
                hand.sprite = hand_run_down[index];
            }
        }
        else if (side == 1)
        {
            bodyskin.sprite = bodyskin_run_up[index];
            if (head_run_up.Length > 0)
            {
                head.sprite = head_run_up[index];
            }
            if (body_run_up.Length > 0)
            {
                body.sprite = body_run_up[index];
            }
            if (pants_run_up.Length > 0)
            {
                pants.sprite = pants_run_up[index];
            }
            if (backpack_run_up.Length > 0)
            {
                backpack.sprite = backpack_run_up[index];
            }
            if (vest_run_up.Length > 0)
            {
                vest.sprite = vest_run_up[index];
            }
            if (hand_run_up.Length > 0)
            {
                hand.sprite = hand_run_up[index];
            }
        }
        else if (side == 2)
        {
            bodyskin.sprite = bodyskin_run_right[index];
            if (head_run_right.Length > 0)
            {
                head.sprite = head_run_right[index];
            }
            if (body_run_right.Length > 0)
            {
                body.sprite = body_run_right[index];
            }
            if (pants_run_right.Length > 0)
            {
                pants.sprite = pants_run_right[index];
            }
            if (backpack_run_right.Length > 0)
            {
                backpack.sprite = backpack_run_right[index];
            }
            if (vest_run_right.Length > 0)
            {
                vest.sprite = vest_run_right[index];
            }
            if (hand_run_right.Length > 0)
            {
                hand.sprite = hand_run_right[index];
            }
        }
        else if (side == 3)
        {
            bodyskin.sprite = bodyskin_run_left[index];
            if (head_run_left.Length > 0)
            {
                head.sprite = head_run_left[index];
            }
            if (body_run_left.Length > 0)
            {
                body.sprite = body_run_left[index];
            }
            if (pants_run_left.Length > 0)
            {
                pants.sprite = pants_run_left[index];
            }
            if (backpack_run_left.Length > 0)
            {
                backpack.sprite = backpack_run_left[index];
            }
            if (vest_run_left.Length > 0)
            {
                vest.sprite = vest_run_left[index];
            }
            if (hand_run_left.Length > 0)
            {
                hand.sprite = hand_run_left[index];
            }
        }
        index++;
    }
    private void AnimIdle(ref int index)
    {
        int side = GetSideByDir(shoot ? ShootDir : animationDir);
        if (controllerState == AnimationControllerState.none)
        {
            bodyskin.sprite = bodyskin_idle[side];
            if (head_idle.Length > 0)
            {
                head.sprite = head_idle[side];
            }
            if (body_idle.Length > 0)
            {
                body.sprite = body_idle[side];
            }
            if (pants_idle.Length > 0)
            {
                pants.sprite = pants_idle[side];
            }
            if (backpack_idle.Length > 0)
            {
                backpack.sprite = backpack_idle[side];
            }
            if (vest_idle.Length > 0)
            {
                vest.sprite = vest_idle[side];
            }
            if (hand_idle.Length > 0)
            {
                hand.sprite = hand_idle[side];
            }
        }
        else
        {
            if (!shoot)
            {
                bodyskin.sprite = bodyskin_weapon_idle[side];
                if (head_idle.Length > 0)
                {
                    head.sprite = head_weapon_idle[side];
                }
                if (body_idle.Length > 0)
                {
                    body.sprite = body_weapon_idle[side];
                }
                if (pants_idle.Length > 0)
                {
                    pants.sprite = pants_weapon_idle[side];
                }
                if (backpack_idle.Length > 0)
                {
                    backpack.sprite = backpack_weapon_idle[side];
                }
                if (vest_idle.Length > 0)
                {
                    vest.sprite = vest_weapon_idle[side];
                }
                if (hand_idle.Length > 0)
                {
                    hand.sprite = hand_weapon_idle[side];
                }
            }
            else
            {
                bodyskin.sprite = bodyskin_shoot[side];
                if (head_idle.Length > 0)
                {
                    head.sprite = head_shoot[side];
                }
                if (body_idle.Length > 0)
                {
                    body.sprite = body_shoot[side];
                }
                if (pants_idle.Length > 0)
                {
                    pants.sprite = pants_shoot[side];
                }
                if (backpack_idle.Length > 0)
                {
                    backpack.sprite = backpack_shoot[side];
                }
                if (vest_idle.Length > 0)
                {
                    vest.sprite = vest_shoot[side];
                }
                if (hand_idle.Length > 0)
                {
                    hand.sprite = hand_shoot[side];
                }
            }
        }
        index++;
    }
    private int GetSideByDir(Vector2 dir) //down up, right left
    {
        if(Mathf.Abs(dir.x) >= Mathf.Abs(dir.y))
        {
            //right left

            if(dir.x >= 0)
            {
                return 2;
            }
            else
            {
                return 3;
            }
        }
        else
        {
            //down up

            if (dir.y >= 0)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }

    public void Shoot(Vector2 dir)
    {
        shoot = true;
        ShootDir = dir;
    }
    public bool Interact(float time = 1)
    {
        if (interact) { return false; }
        playerNetwork.GetComponent<PlayerMove>().enabled = false;
        playerNetwork.GetComponent<WeaponController>().itsEnable = false;
        StartCoroutine(IEInteract(time));
        return true;
    }

    #region AnimationSetter

    #region Vars
    [Header("Body Skin")]
    public Sprite[] bodyskin_idle;
    public Sprite[] bodyskin_run_down;
    public Sprite[] bodyskin_run_up;
    public Sprite[] bodyskin_run_right;
    public Sprite[] bodyskin_run_left;
    public Sprite[] bodyskin_interact;
    public Sprite[] bodyskin_shoot;
    public Sprite[] bodyskin_weapon_idle;
    public Sprite[] bodyskin_pistol_idle;
    [Header("Head")]
    public Sprite[] head_idle;
    public Sprite[] head_run_down;
    public Sprite[] head_run_up;
    public Sprite[] head_run_right;
    public Sprite[] head_run_left;
    public Sprite[] head_interact;
    public Sprite[] head_shoot;
    public Sprite[] head_weapon_idle;
    [Header("Body")]
    public Sprite[] body_idle;
    public Sprite[] body_run_down;
    public Sprite[] body_run_up;
    public Sprite[] body_run_right;
    public Sprite[] body_run_left;
    public Sprite[] body_interact;
    public Sprite[] body_shoot;
    public Sprite[] body_weapon_idle;
    public Sprite[] body_pistol_idle;
    [Header("Pants")]
    public Sprite[] pants_idle;
    public Sprite[] pants_run_down;
    public Sprite[] pants_run_up;
    public Sprite[] pants_run_right;
    public Sprite[] pants_run_left;
    public Sprite[] pants_interact;
    public Sprite[] pants_shoot;
    public Sprite[] pants_weapon_idle;
    [Header("Backpack")]
    public Sprite[] backpack_idle;
    public Sprite[] backpack_run_down;
    public Sprite[] backpack_run_up;
    public Sprite[] backpack_run_right;
    public Sprite[] backpack_run_left;
    public Sprite[] backpack_interact;
    public Sprite[] backpack_shoot;
    public Sprite[] backpack_weapon_idle;
    [Header("Hand")]
    public Sprite[] hand_idle;
    public Sprite[] hand_run_down;
    public Sprite[] hand_run_up;
    public Sprite[] hand_run_right;
    public Sprite[] hand_run_left;
    public Sprite[] hand_interact;
    public Sprite[] hand_shoot;
    public Sprite[] hand_weapon_idle;
    public Sprite[] hand_pistol_idle;
    [Header("Vest")]
    public Sprite[] vest_idle;
    public Sprite[] vest_run_down;
    public Sprite[] vest_run_up;
    public Sprite[] vest_run_right;
    public Sprite[] vest_run_left;
    public Sprite[] vest_interact;
    public Sprite[] vest_shoot;
    public Sprite[] vest_weapon_idle;

    #endregion

    public void SetClothByName(string itemname, bool isWeapon = false)
    {
        AnimationItem animationItem = (AnimationItem)Resources.Load(path_to_items + itemname);

        if (animationItem == null) { Debug.LogError(itemname + " is NULL!"); return; }
        controllerState = isWeapon ? AnimationControllerState.rifle : AnimationControllerState.none;
        if (animationItem.type == AnimationThpe.bodyskin)
        {
            cur_bodyskin = itemname;
            //idle, interact
            for (int i = 0; i < bodyskin_idle.Length; i++)
            {
                bodyskin_idle[i] = animationItem.idle[i];
            }
            for (int i = 0; i < bodyskin_interact.Length; i++)
            {
                bodyskin_interact[i] = animationItem.interact[i];
            }
            //run
            for (int i = 0; i < bodyskin_run_down.Length; i++)
            {
                bodyskin_run_down[i] = animationItem.run_down[i];
            }
            for (int i = 0; i < bodyskin_run_up.Length; i++)
            {
                bodyskin_run_up[i] = animationItem.run_up[i];
            }
            for (int i = 0; i < bodyskin_run_right.Length; i++)
            {
                bodyskin_run_right[i] = animationItem.run_right[i];
            }
            for (int i = 0; i < bodyskin_run_left.Length; i++)
            {
                bodyskin_run_left[i] = animationItem.run_left[i];
            }
            //weapon
            for (int i = 0; i < bodyskin_weapon_idle.Length; i++)
            {
                bodyskin_weapon_idle[i] = animationItem.weapon_idle[i];
            }
            for (int i = 0; i < bodyskin_pistol_idle.Length; i++)
            {
                bodyskin_pistol_idle[i] = animationItem.pistol_idle[i];
            }
            for (int i = 0; i < bodyskin_shoot.Length; i++)
            {
                bodyskin_shoot[i] = animationItem.shoot[i];
            }
        }
        if (animationItem.type == AnimationThpe.head)
        {
            cur_head = itemname;
            //idle, interact
            for (int i = 0; i < head_idle.Length; i++)
            {
                head_idle[i] = animationItem.idle[i];
            }
            for (int i = 0; i < head_interact.Length; i++)
            {
                head_interact[i] = animationItem.interact[i];
            }
            //run
            for (int i = 0; i < head_run_down.Length; i++)
            {
                head_run_down[i] = animationItem.run_down[i];
            }
            for (int i = 0; i < head_run_up.Length; i++)
            {
                head_run_up[i] = animationItem.run_up[i];
            }
            for (int i = 0; i < head_run_right.Length; i++)
            {
                head_run_right[i] = animationItem.run_right[i];
            }
            for (int i = 0; i < head_run_left.Length; i++)
            {
                head_run_left[i] = animationItem.run_left[i];
            }
            //weapon
            for (int i = 0; i < head_weapon_idle.Length; i++)
            {
                head_weapon_idle[i] = animationItem.weapon_idle[i];
            }
            for (int i = 0; i < head_shoot.Length; i++)
            {
                head_shoot[i] = animationItem.shoot[i];
            }
        }
        if (animationItem.type == AnimationThpe.body)
        {
            cur_body = itemname;
            //idle, interact
            for (int i = 0; i < body_idle.Length; i++)
            {
                body_idle[i] = animationItem.idle[i];
            }
            for (int i = 0; i < body_interact.Length; i++)
            {
                body_interact[i] = animationItem.interact[i];
            }
            //run
            for (int i = 0; i < body_run_down.Length; i++)
            {
                body_run_down[i] = animationItem.run_down[i];
            }
            for (int i = 0; i < body_run_up.Length; i++)
            {
                body_run_up[i] = animationItem.run_up[i];
            }
            for (int i = 0; i < body_run_right.Length; i++)
            {
                body_run_right[i] = animationItem.run_right[i];
            }
            for (int i = 0; i < body_run_left.Length; i++)
            {
                body_run_left[i] = animationItem.run_left[i];
            }
            //weapon
            for (int i = 0; i < body_weapon_idle.Length; i++)
            {
                body_weapon_idle[i] = animationItem.weapon_idle[i];
            }
            for (int i = 0; i < body_pistol_idle.Length; i++)
            {
                body_pistol_idle[i] = animationItem.pistol_idle[i];
            }
            for (int i = 0; i < body_shoot.Length; i++)
            {
                body_shoot[i] = animationItem.shoot[i];
            }
        }
        if (animationItem.type == AnimationThpe.pants)
        {
            cur_pants = itemname;
            //idle, interact
            for (int i = 0; i < pants_idle.Length; i++)
            {
                pants_idle[i] = animationItem.idle[i];
            }
            for (int i = 0; i < pants_interact.Length; i++)
            {
                pants_interact[i] = animationItem.interact[i];
            }
            //run
            for (int i = 0; i < pants_run_down.Length; i++)
            {
                pants_run_down[i] = animationItem.run_down[i];
            }
            for (int i = 0; i < pants_run_up.Length; i++)
            {
                pants_run_up[i] = animationItem.run_up[i];
            }
            for (int i = 0; i < pants_run_right.Length; i++)
            {
                pants_run_right[i] = animationItem.run_right[i];
            }
            for (int i = 0; i < pants_run_left.Length; i++)
            {
                pants_run_left[i] = animationItem.run_left[i];
            }
            //weapon
            for (int i = 0; i < pants_weapon_idle.Length; i++)
            {
                pants_weapon_idle[i] = animationItem.weapon_idle[i];
            }
            for (int i = 0; i < pants_shoot.Length; i++)
            {
                pants_shoot[i] = animationItem.shoot[i];
            }
        }
        if (animationItem.type == AnimationThpe.backpack)
        {
            cur_backpack = itemname;
            //idle, interact
            for (int i = 0; i < backpack_idle.Length; i++)
            {
                backpack_idle[i] = animationItem.idle[i];
            }
            for (int i = 0; i < backpack_interact.Length; i++)
            {
                backpack_interact[i] = animationItem.interact[i];
            }
            //run
            for (int i = 0; i < backpack_run_down.Length; i++)
            {
                backpack_run_down[i] = animationItem.run_down[i];
            }
            for (int i = 0; i < backpack_run_up.Length; i++)
            {
                backpack_run_up[i] = animationItem.run_up[i];
            }
            for (int i = 0; i < backpack_run_right.Length; i++)
            {
                backpack_run_right[i] = animationItem.run_right[i];
            }
            for (int i = 0; i < backpack_run_left.Length; i++)
            {
                backpack_run_left[i] = animationItem.run_left[i];
            }
            //weapon
            for (int i = 0; i < backpack_weapon_idle.Length; i++)
            {
                backpack_weapon_idle[i] = animationItem.weapon_idle[i];
            }
            for (int i = 0; i < backpack_shoot.Length; i++)
            {
                backpack_shoot[i] = animationItem.shoot[i];

            }
        }
        if (animationItem.type == AnimationThpe.hand)
        {
            cur_hand = itemname;
            //idle, interact
            for (int i = 0; i < hand_idle.Length; i++)
            {
                hand_idle[i] = animationItem.idle[i];
            }
            for (int i = 0; i < hand_interact.Length; i++)
            {
                hand_interact[i] = animationItem.interact[i];
            }
            //run
            for (int i = 0; i < hand_run_down.Length; i++)
            {
                hand_run_down[i] = animationItem.run_down[i];
            }
            for (int i = 0; i < hand_run_up.Length; i++)
            {
                hand_run_up[i] = animationItem.run_up[i];
            }
            for (int i = 0; i < hand_run_right.Length; i++)
            {
                hand_run_right[i] = animationItem.run_right[i];
            }
            for (int i = 0; i < hand_run_left.Length; i++)
            {
                hand_run_left[i] = animationItem.run_left[i];
            }
            //weapon
            for (int i = 0; i < hand_weapon_idle.Length; i++)
            {
                hand_weapon_idle[i] = animationItem.weapon_idle[i];
            }
            for (int i = 0; i < hand_pistol_idle.Length; i++)
            {
                hand_pistol_idle[i] = animationItem.pistol_idle[i];
            }
            for (int i = 0; i < hand_shoot.Length; i++)
            {
                hand_shoot[i] = animationItem.shoot[i];
            }
        }
        if (animationItem.type == AnimationThpe.vest)
        {
            cur_vest = itemname;
            //idle, interact
            for (int i = 0; i < vest_idle.Length; i++)
            {
                vest_idle[i] = animationItem.idle[i];
            }
            for (int i = 0; i < vest_interact.Length; i++)
            {
                vest_interact[i] = animationItem.interact[i];
            }
            //run
            for (int i = 0; i < vest_run_down.Length; i++)
            {
                vest_run_down[i] = animationItem.run_down[i];
            }
            for (int i = 0; i < vest_run_up.Length; i++)
            {
                vest_run_up[i] = animationItem.run_up[i];
            }
            for (int i = 0; i < vest_run_right.Length; i++)
            {
                vest_run_right[i] = animationItem.run_right[i];
            }
            for (int i = 0; i < vest_run_left.Length; i++)
            {
                vest_run_left[i] = animationItem.run_left[i];
            }
            //weapon
            for (int i = 0; i < vest_weapon_idle.Length; i++)
            {
                vest_weapon_idle[i] = animationItem.weapon_idle[i];
            }
            for (int i = 0; i < vest_shoot.Length; i++)
            {
                vest_shoot[i] = animationItem.shoot[i];
            }
        }

        NetUpdateMyClotheData();
    }
    public void SetNetClothByName(string itemname, bool isWeapon = false)
    {
        //print(transform.parent.name + " надел: " + itemname);
        AnimationItem animationItem = (AnimationItem)Resources.Load(path_to_items + itemname);
        controllerState = isWeapon ? AnimationControllerState.rifle : AnimationControllerState.none;
        if (animationItem == null) { return; }
        if (animationItem.type == AnimationThpe.bodyskin)
        {
            cur_bodyskin = itemname;
            //idle, interact
            for (int i = 0; i < bodyskin_idle.Length; i++)
            {
                bodyskin_idle[i] = animationItem.idle[i];
            }
            for (int i = 0; i < bodyskin_interact.Length; i++)
            {
                bodyskin_interact[i] = animationItem.interact[i];
            }
            //run
            for (int i = 0; i < bodyskin_run_down.Length; i++)
            {
                bodyskin_run_down[i] = animationItem.run_down[i];
            }
            for (int i = 0; i < bodyskin_run_up.Length; i++)
            {
                bodyskin_run_up[i] = animationItem.run_up[i];
            }
            for (int i = 0; i < bodyskin_run_right.Length; i++)
            {
                bodyskin_run_right[i] = animationItem.run_right[i];
            }
            for (int i = 0; i < bodyskin_run_left.Length; i++)
            {
                bodyskin_run_left[i] = animationItem.run_left[i];
            }
            //weapon
            for (int i = 0; i < bodyskin_weapon_idle.Length; i++)
            {
                bodyskin_weapon_idle[i] = animationItem.weapon_idle[i];
            }
            for (int i = 0; i < bodyskin_pistol_idle.Length; i++)
            {
                bodyskin_pistol_idle[i] = animationItem.pistol_idle[i];
            }
            for (int i = 0; i < bodyskin_shoot.Length; i++)
            {
                bodyskin_shoot[i] = animationItem.shoot[i];
            }
        }
        if (animationItem.type == AnimationThpe.head)
        {
            cur_head = itemname;
            //idle, interact
            for (int i = 0; i < head_idle.Length; i++)
            {
                head_idle[i] = animationItem.idle[i];
            }
            for (int i = 0; i < head_interact.Length; i++)
            {
                head_interact[i] = animationItem.interact[i];
            }
            //run
            for (int i = 0; i < head_run_down.Length; i++)
            {
                head_run_down[i] = animationItem.run_down[i];
            }
            for (int i = 0; i < head_run_up.Length; i++)
            {
                head_run_up[i] = animationItem.run_up[i];
            }
            for (int i = 0; i < head_run_right.Length; i++)
            {
                head_run_right[i] = animationItem.run_right[i];
            }
            for (int i = 0; i < head_run_left.Length; i++)
            {
                head_run_left[i] = animationItem.run_left[i];
            }
            //weapon
            for (int i = 0; i < head_weapon_idle.Length; i++)
            {
                head_weapon_idle[i] = animationItem.weapon_idle[i];
            }
            for (int i = 0; i < head_shoot.Length; i++)
            {
                head_shoot[i] = animationItem.shoot[i];
            }
        }
        if (animationItem.type == AnimationThpe.body)
        {
            cur_body = itemname;
            //idle, interact
            for (int i = 0; i < body_idle.Length; i++)
            {
                body_idle[i] = animationItem.idle[i];
            }
            for (int i = 0; i < body_interact.Length; i++)
            {
                body_interact[i] = animationItem.interact[i];
            }
            //run
            for (int i = 0; i < body_run_down.Length; i++)
            {
                body_run_down[i] = animationItem.run_down[i];
            }
            for (int i = 0; i < body_run_up.Length; i++)
            {
                body_run_up[i] = animationItem.run_up[i];
            }
            for (int i = 0; i < body_run_right.Length; i++)
            {
                body_run_right[i] = animationItem.run_right[i];
            }
            for (int i = 0; i < body_run_left.Length; i++)
            {
                body_run_left[i] = animationItem.run_left[i];
            }
            //weapon
            for (int i = 0; i < body_weapon_idle.Length; i++)
            {
                body_weapon_idle[i] = animationItem.weapon_idle[i];
            }
            for (int i = 0; i < body_pistol_idle.Length; i++)
            {
                body_pistol_idle[i] = animationItem.pistol_idle[i];
            }
            for (int i = 0; i < body_shoot.Length; i++)
            {
                body_shoot[i] = animationItem.shoot[i];
            }
        }
        if (animationItem.type == AnimationThpe.pants)
        {
            cur_pants = itemname;
            //idle, interact
            for (int i = 0; i < pants_idle.Length; i++)
            {
                pants_idle[i] = animationItem.idle[i];
            }
            for (int i = 0; i < pants_interact.Length; i++)
            {
                pants_interact[i] = animationItem.interact[i];
            }
            //run
            for (int i = 0; i < pants_run_down.Length; i++)
            {
                pants_run_down[i] = animationItem.run_down[i];
            }
            for (int i = 0; i < pants_run_up.Length; i++)
            {
                pants_run_up[i] = animationItem.run_up[i];
            }
            for (int i = 0; i < pants_run_right.Length; i++)
            {
                pants_run_right[i] = animationItem.run_right[i];
            }
            for (int i = 0; i < pants_run_left.Length; i++)
            {
                pants_run_left[i] = animationItem.run_left[i];
            }
            //weapon
            for (int i = 0; i < pants_weapon_idle.Length; i++)
            {
                pants_weapon_idle[i] = animationItem.weapon_idle[i];
            }
            for (int i = 0; i < pants_shoot.Length; i++)
            {
                pants_shoot[i] = animationItem.shoot[i];
            }
        }
        if (animationItem.type == AnimationThpe.backpack)
        {
            cur_backpack = itemname;
            //idle, interact
            for (int i = 0; i < backpack_idle.Length; i++)
            {
                backpack_idle[i] = animationItem.idle[i];
            }
            for (int i = 0; i < backpack_interact.Length; i++)
            {
                backpack_interact[i] = animationItem.interact[i];
            }
            //run
            for (int i = 0; i < backpack_run_down.Length; i++)
            {
                backpack_run_down[i] = animationItem.run_down[i];
            }
            for (int i = 0; i < backpack_run_up.Length; i++)
            {
                backpack_run_up[i] = animationItem.run_up[i];
            }
            for (int i = 0; i < backpack_run_right.Length; i++)
            {
                backpack_run_right[i] = animationItem.run_right[i];
            }
            for (int i = 0; i < backpack_run_left.Length; i++)
            {
                backpack_run_left[i] = animationItem.run_left[i];
            }
            //weapon
            for (int i = 0; i < backpack_weapon_idle.Length; i++)
            {
                backpack_weapon_idle[i] = animationItem.weapon_idle[i];
            }
            for (int i = 0; i < backpack_shoot.Length; i++)
            {
                backpack_shoot[i] = animationItem.shoot[i];

            }
        }
        if (animationItem.type == AnimationThpe.hand)
        {
            cur_hand = itemname;
            //idle, interact
            for (int i = 0; i < hand_idle.Length; i++)
            {
                hand_idle[i] = animationItem.idle[i];
            }
            for (int i = 0; i < hand_interact.Length; i++)
            {
                hand_interact[i] = animationItem.interact[i];
            }
            //run
            for (int i = 0; i < hand_run_down.Length; i++)
            {
                hand_run_down[i] = animationItem.run_down[i];
            }
            for (int i = 0; i < hand_run_up.Length; i++)
            {
                hand_run_up[i] = animationItem.run_up[i];
            }
            for (int i = 0; i < hand_run_right.Length; i++)
            {
                hand_run_right[i] = animationItem.run_right[i];
            }
            for (int i = 0; i < hand_run_left.Length; i++)
            {
                hand_run_left[i] = animationItem.run_left[i];
            }
            //weapon
            for (int i = 0; i < hand_weapon_idle.Length; i++)
            {
                hand_weapon_idle[i] = animationItem.weapon_idle[i];
            }
            for (int i = 0; i < hand_pistol_idle.Length; i++)
            {
                hand_pistol_idle[i] = animationItem.pistol_idle[i];
            }
            for (int i = 0; i < hand_shoot.Length; i++)
            {
                hand_shoot[i] = animationItem.shoot[i];
            }
        }
        if (animationItem.type == AnimationThpe.vest)
        {
            cur_vest = itemname;
            //idle, interact
            for (int i = 0; i < vest_idle.Length; i++)
            {
                vest_idle[i] = animationItem.idle[i];
            }
            for (int i = 0; i < vest_interact.Length; i++)
            {
                vest_interact[i] = animationItem.interact[i];
            }
            //run
            for (int i = 0; i < vest_run_down.Length; i++)
            {
                vest_run_down[i] = animationItem.run_down[i];
            }
            for (int i = 0; i < vest_run_up.Length; i++)
            {
                vest_run_up[i] = animationItem.run_up[i];
            }
            for (int i = 0; i < vest_run_right.Length; i++)
            {
                vest_run_right[i] = animationItem.run_right[i];
            }
            for (int i = 0; i < vest_run_left.Length; i++)
            {
                vest_run_left[i] = animationItem.run_left[i];
            }
            //weapon
            for (int i = 0; i < vest_weapon_idle.Length; i++)
            {
                vest_weapon_idle[i] = animationItem.weapon_idle[i];
            }
            for (int i = 0; i < vest_shoot.Length; i++)
            {
                vest_shoot[i] = animationItem.shoot[i];
            }
        }
    }
    public void ClearNetClothByType(AnimationThpe animType)
    {
        //print(animType + " clearning...");
        controllerState = AnimationControllerState.none;
        if (animType == AnimationThpe.bodyskin)
        {
            //idle, interact
            for (int i = 0; i < bodyskin_idle.Length; i++)
            {
                bodyskin_idle[i] = null;
            }
            for (int i = 0; i < bodyskin_interact.Length; i++)
            {
                bodyskin_interact[i] = null;
            }
            //run
            for (int i = 0; i < bodyskin_run_down.Length; i++)
            {
                bodyskin_run_down[i] = null;
            }
            for (int i = 0; i < bodyskin_run_up.Length; i++)
            {
                bodyskin_run_up[i] = null;
            }
            for (int i = 0; i < bodyskin_run_right.Length; i++)
            {
                bodyskin_run_right[i] = null;
            }
            for (int i = 0; i < bodyskin_run_left.Length; i++)
            {
                bodyskin_run_left[i] = null;
            }
            //weapon
            for (int i = 0; i < bodyskin_weapon_idle.Length; i++)
            {
                bodyskin_weapon_idle[i] = null;
            }
            for (int i = 0; i < bodyskin_pistol_idle.Length; i++)
            {
                bodyskin_pistol_idle[i] = null;
            }
            for (int i = 0; i < bodyskin_shoot.Length; i++)
            {
                bodyskin_shoot[i] = null;
            }
        }
        if (animType == AnimationThpe.head)
        {
            //idle, interact
            for (int i = 0; i < head_idle.Length; i++)
            {
                head_idle[i] = null;
            }
            for (int i = 0; i < head_interact.Length; i++)
            {
                head_interact[i] = null;
            }
            //run
            for (int i = 0; i < head_run_down.Length; i++)
            {
                head_run_down[i] = null;
            }
            for (int i = 0; i < head_run_up.Length; i++)
            {
                head_run_up[i] = null;
            }
            for (int i = 0; i < head_run_right.Length; i++)
            {
                head_run_right[i] = null;
            }
            for (int i = 0; i < head_run_left.Length; i++)
            {
                head_run_left[i] = null;
            }
            //weapon
            for (int i = 0; i < head_weapon_idle.Length; i++)
            {
                head_weapon_idle[i] = null;
            }
            for (int i = 0; i < head_shoot.Length; i++)
            {
                head_shoot[i] = null;
            }
        }
        if (animType == AnimationThpe.body)
        {
            //idle, interact
            for (int i = 0; i < body_idle.Length; i++)
            {
                body_idle[i] = null;
            }
            for (int i = 0; i < body_interact.Length; i++)
            {
                body_interact[i] = null;
            }
            //run
            for (int i = 0; i < body_run_down.Length; i++)
            {
                body_run_down[i] = null;
            }
            for (int i = 0; i < body_run_up.Length; i++)
            {
                body_run_up[i] = null;
            }
            for (int i = 0; i < body_run_right.Length; i++)
            {
                body_run_right[i] = null;
            }
            for (int i = 0; i < body_run_left.Length; i++)
            {
                body_run_left[i] = null;
            }
            //weapon
            for (int i = 0; i < body_weapon_idle.Length; i++)
            {
                body_weapon_idle[i] = null;
            }
            for (int i = 0; i < body_pistol_idle.Length; i++)
            {
                body_pistol_idle[i] = null;
            }
            for (int i = 0; i < body_shoot.Length; i++)
            {
                body_shoot[i] = null;
            }
        }
        if (animType == AnimationThpe.pants)
        {
            //idle, interact
            for (int i = 0; i < pants_idle.Length; i++)
            {
                pants_idle[i] = null;
            }
            for (int i = 0; i < pants_interact.Length; i++)
            {
                pants_interact[i] = null;
            }
            //run
            for (int i = 0; i < pants_run_down.Length; i++)
            {
                pants_run_down[i] = null;
            }
            for (int i = 0; i < pants_run_up.Length; i++)
            {
                pants_run_up[i] = null;
            }
            for (int i = 0; i < pants_run_right.Length; i++)
            {
                pants_run_right[i] = null;
            }
            for (int i = 0; i < pants_run_left.Length; i++)
            {
                pants_run_left[i] = null;
            }
            //weapon
            for (int i = 0; i < pants_weapon_idle.Length; i++)
            {
                pants_weapon_idle[i] = null;
            }
            for (int i = 0; i < pants_shoot.Length; i++)
            {
                pants_shoot[i] = null;
            }
        }
        if (animType == AnimationThpe.backpack)
        {
            //idle, interact
            for (int i = 0; i < backpack_idle.Length; i++)
            {
                backpack_idle[i] = null;
            }
            for (int i = 0; i < backpack_interact.Length; i++)
            {
                backpack_interact[i] = null;
            }
            //run
            for (int i = 0; i < backpack_run_down.Length; i++)
            {
                backpack_run_down[i] = null;
            }
            for (int i = 0; i < backpack_run_up.Length; i++)
            {
                backpack_run_up[i] = null;
            }
            for (int i = 0; i < backpack_run_right.Length; i++)
            {
                backpack_run_right[i] = null;
            }
            for (int i = 0; i < backpack_run_left.Length; i++)
            {
                backpack_run_left[i] = null;
            }
            //weapon
            for (int i = 0; i < backpack_weapon_idle.Length; i++)
            {
                backpack_weapon_idle[i] = null;
            }
            for (int i = 0; i < backpack_shoot.Length; i++)
            {
                backpack_shoot[i] = null;

            }
        }
        if (animType == AnimationThpe.hand)
        {
            //idle, interact
            for (int i = 0; i < hand_idle.Length; i++)
            {
                hand_idle[i] = null;
            }
            for (int i = 0; i < hand_interact.Length; i++)
            {
                hand_interact[i] = null;
            }
            //run
            for (int i = 0; i < hand_run_down.Length; i++)
            {
                hand_run_down[i] = null;
            }
            for (int i = 0; i < hand_run_up.Length; i++)
            {
                hand_run_up[i] = null;
            }
            for (int i = 0; i < hand_run_right.Length; i++)
            {
                hand_run_right[i] = null;
            }
            for (int i = 0; i < hand_run_left.Length; i++)
            {
                hand_run_left[i] = null;
            }
            //weapon
            for (int i = 0; i < hand_weapon_idle.Length; i++)
            {
                hand_weapon_idle[i] = null;
            }
            for (int i = 0; i < hand_pistol_idle.Length; i++)
            {
                hand_pistol_idle[i] = null;
            }
            for (int i = 0; i < hand_shoot.Length; i++)
            {
                hand_shoot[i] = null;
            }
        }
        if (animType == AnimationThpe.vest)
        {
            //idle, interact
            for (int i = 0; i < vest_idle.Length; i++)
            {
                vest_idle[i] = null;
            }
            for (int i = 0; i < vest_interact.Length; i++)
            {
                vest_interact[i] = null;
            }
            //run
            for (int i = 0; i < vest_run_down.Length; i++)
            {
                vest_run_down[i] = null;
            }
            for (int i = 0; i < vest_run_up.Length; i++)
            {
                vest_run_up[i] = null;
            }
            for (int i = 0; i < vest_run_right.Length; i++)
            {
                vest_run_right[i] = null;
            }
            for (int i = 0; i < vest_run_left.Length; i++)
            {
                vest_run_left[i] = null;
            }
            //weapon
            for (int i = 0; i < vest_weapon_idle.Length; i++)
            {
                vest_weapon_idle[i] = null;
            }
            for (int i = 0; i < vest_shoot.Length; i++)
            {
                vest_shoot[i] = null;
            }
        }
    }
    public void ClearClothByName(string itemname)
    {
        AnimationItem animationItem = (AnimationItem)Resources.Load(path_to_items + itemname);
        controllerState = AnimationControllerState.none;
        if (animationItem == null) { return; }
        //print(animationItem.name + " clearning...");
        if (animationItem.type == AnimationThpe.bodyskin)
        {
            cur_bodyskin = null;
            //idle, interact
            for (int i = 0; i < bodyskin_idle.Length; i++)
            {
                bodyskin_idle[i] = null;
            }
            for (int i = 0; i < bodyskin_interact.Length; i++)
            {
                bodyskin_interact[i] = null;
            }
            //run
            for (int i = 0; i < bodyskin_run_down.Length; i++)
            {
                bodyskin_run_down[i] = null;
            }
            for (int i = 0; i < bodyskin_run_up.Length; i++)
            {
                bodyskin_run_up[i] = null;
            }
            for (int i = 0; i < bodyskin_run_right.Length; i++)
            {
                bodyskin_run_right[i] = null;
            }
            for (int i = 0; i < bodyskin_run_left.Length; i++)
            {
                bodyskin_run_left[i] = null;
            }
            //weapon
            for (int i = 0; i < bodyskin_weapon_idle.Length; i++)
            {
                bodyskin_weapon_idle[i] = null;
            }
            for (int i = 0; i < bodyskin_pistol_idle.Length; i++)
            {
                bodyskin_pistol_idle[i] = null;
            }
            for (int i = 0; i < bodyskin_shoot.Length; i++)
            {
                bodyskin_shoot[i] = null;
            }
        }
        if (animationItem.type == AnimationThpe.head)
        {
            cur_head = null;
            //idle, interact
            for (int i = 0; i < head_idle.Length; i++)
            {
                head_idle[i] = null;
            }
            for (int i = 0; i < head_interact.Length; i++)
            {
                head_interact[i] = null;
            }
            //run
            for (int i = 0; i < head_run_down.Length; i++)
            {
                head_run_down[i] = null;
            }
            for (int i = 0; i < head_run_up.Length; i++)
            {
                head_run_up[i] = null;
            }
            for (int i = 0; i < head_run_right.Length; i++)
            {
                head_run_right[i] = null;
            }
            for (int i = 0; i < head_run_left.Length; i++)
            {
                head_run_left[i] = null;
            }
            //weapon
            for (int i = 0; i < head_weapon_idle.Length; i++)
            {
                head_weapon_idle[i] = null;
            }
            for (int i = 0; i < head_shoot.Length; i++)
            {
                head_shoot[i] = null;
            }
        }
        if (animationItem.type == AnimationThpe.body)
        {
            cur_body = null;
            //idle, interact
            for (int i = 0; i < body_idle.Length; i++)
            {
                body_idle[i] = null;
            }
            for (int i = 0; i < body_interact.Length; i++)
            {
                body_interact[i] = null;
            }
            //run
            for (int i = 0; i < body_run_down.Length; i++)
            {
                body_run_down[i] = null;
            }
            for (int i = 0; i < body_run_up.Length; i++)
            {
                body_run_up[i] = null;
            }
            for (int i = 0; i < body_run_right.Length; i++)
            {
                body_run_right[i] = null;
            }
            for (int i = 0; i < body_run_left.Length; i++)
            {
                body_run_left[i] = null;
            }
            //weapon
            for (int i = 0; i < body_weapon_idle.Length; i++)
            {
                body_weapon_idle[i] = null;
            }
            for (int i = 0; i < body_pistol_idle.Length; i++)
            {
                body_pistol_idle[i] = null;
            }
            for (int i = 0; i < body_shoot.Length; i++)
            {
                body_shoot[i] = null;
            }
        }
        if (animationItem.type == AnimationThpe.pants)
        {
            cur_pants = null;
            //idle, interact
            for (int i = 0; i < pants_idle.Length; i++)
            {
                pants_idle[i] = null;
            }
            for (int i = 0; i < pants_interact.Length; i++)
            {
                pants_interact[i] = null;
            }
            //run
            for (int i = 0; i < pants_run_down.Length; i++)
            {
                pants_run_down[i] = null;
            }
            for (int i = 0; i < pants_run_up.Length; i++)
            {
                pants_run_up[i] = null;
            }
            for (int i = 0; i < pants_run_right.Length; i++)
            {
                pants_run_right[i] = null;
            }
            for (int i = 0; i < pants_run_left.Length; i++)
            {
                pants_run_left[i] = null;
            }
            //weapon
            for (int i = 0; i < pants_weapon_idle.Length; i++)
            {
                pants_weapon_idle[i] = null;
            }
            for (int i = 0; i < pants_shoot.Length; i++)
            {
                pants_shoot[i] = null;
            }
        }
        if (animationItem.type == AnimationThpe.backpack)
        {
            cur_backpack = null;
            //idle, interact
            for (int i = 0; i < backpack_idle.Length; i++)
            {
                backpack_idle[i] = null;
            }
            for (int i = 0; i < backpack_interact.Length; i++)
            {
                backpack_interact[i] = null;
            }
            //run
            for (int i = 0; i < backpack_run_down.Length; i++)
            {
                backpack_run_down[i] = null;
            }
            for (int i = 0; i < backpack_run_up.Length; i++)
            {
                backpack_run_up[i] = null;
            }
            for (int i = 0; i < backpack_run_right.Length; i++)
            {
                backpack_run_right[i] = null;
            }
            for (int i = 0; i < backpack_run_left.Length; i++)
            {
                backpack_run_left[i] = null;
            }
            //weapon
            for (int i = 0; i < backpack_weapon_idle.Length; i++)
            {
                backpack_weapon_idle[i] = null;
            }
            for (int i = 0; i < backpack_shoot.Length; i++)
            {
                backpack_shoot[i] = null;

            }
        }
        if (animationItem.type == AnimationThpe.hand)
        {
            cur_hand = null;
            //idle, interact
            for (int i = 0; i < hand_idle.Length; i++)
            {
                hand_idle[i] = null;
            }
            for (int i = 0; i < hand_interact.Length; i++)
            {
                hand_interact[i] = null;
            }
            //run
            for (int i = 0; i < hand_run_down.Length; i++)
            {
                hand_run_down[i] = null;
            }
            for (int i = 0; i < hand_run_up.Length; i++)
            {
                hand_run_up[i] = null;
            }
            for (int i = 0; i < hand_run_right.Length; i++)
            {
                hand_run_right[i] = null;
            }
            for (int i = 0; i < hand_run_left.Length; i++)
            {
                hand_run_left[i] = null;
            }
            //weapon
            for (int i = 0; i < hand_weapon_idle.Length; i++)
            {
                hand_weapon_idle[i] = null;
            }
            for (int i = 0; i < hand_pistol_idle.Length; i++)
            {
                hand_pistol_idle[i] = null;
            }
            for (int i = 0; i < hand_shoot.Length; i++)
            {
                hand_shoot[i] = null;
            }
        }
        if (animationItem.type == AnimationThpe.vest)
        {
            cur_vest = null;
            //idle, interact
            for (int i = 0; i < vest_idle.Length; i++)
            {
                vest_idle[i] = null;
            }
            for (int i = 0; i < vest_interact.Length; i++)
            {
                vest_interact[i] = null;
            }
            //run
            for (int i = 0; i < vest_run_down.Length; i++)
            {
                vest_run_down[i] = null;
            }
            for (int i = 0; i < vest_run_up.Length; i++)
            {
                vest_run_up[i] = null;
            }
            for (int i = 0; i < vest_run_right.Length; i++)
            {
                vest_run_right[i] = null;
            }
            for (int i = 0; i < vest_run_left.Length; i++)
            {
                vest_run_left[i] = null;
            }
            //weapon
            for (int i = 0; i < vest_weapon_idle.Length; i++)
            {
                vest_weapon_idle[i] = null;
            }
            for (int i = 0; i < vest_shoot.Length; i++)
            {
                vest_shoot[i] = null;
            }
        }

        NetUpdateMyClotheData();
    }
    public void ClearClothByType(AnimationThpe animType)
    {
        controllerState = AnimationControllerState.none;
        //print(animType + " clearning...");
        if (animType == AnimationThpe.bodyskin)
        {
            cur_bodyskin = null;
            //idle, interact
            for (int i = 0; i < bodyskin_idle.Length; i++)
            {
                bodyskin_idle[i] = null;
            }
            for (int i = 0; i < bodyskin_interact.Length; i++)
            {
                bodyskin_interact[i] = null;
            }
            //run
            for (int i = 0; i < bodyskin_run_down.Length; i++)
            {
                bodyskin_run_down[i] = null;
            }
            for (int i = 0; i < bodyskin_run_up.Length; i++)
            {
                bodyskin_run_up[i] = null;
            }
            for (int i = 0; i < bodyskin_run_right.Length; i++)
            {
                bodyskin_run_right[i] = null;
            }
            for (int i = 0; i < bodyskin_run_left.Length; i++)
            {
                bodyskin_run_left[i] = null;
            }
            //weapon
            for (int i = 0; i < bodyskin_weapon_idle.Length; i++)
            {
                bodyskin_weapon_idle[i] = null;
            }
            for (int i = 0; i < bodyskin_pistol_idle.Length; i++)
            {
                bodyskin_pistol_idle[i] = null;
            }
            for (int i = 0; i < bodyskin_shoot.Length; i++)
            {
                bodyskin_shoot[i] = null;
            }
        }
        if (animType == AnimationThpe.head)
        {
            cur_head = null;
            //idle, interact
            for (int i = 0; i < head_idle.Length; i++)
            {
                head_idle[i] = null;
            }
            for (int i = 0; i < head_interact.Length; i++)
            {
                head_interact[i] = null;
            }
            //run
            for (int i = 0; i < head_run_down.Length; i++)
            {
                head_run_down[i] = null;
            }
            for (int i = 0; i < head_run_up.Length; i++)
            {
                head_run_up[i] = null;
            }
            for (int i = 0; i < head_run_right.Length; i++)
            {
                head_run_right[i] = null;
            }
            for (int i = 0; i < head_run_left.Length; i++)
            {
                head_run_left[i] = null;
            }
            //weapon
            for (int i = 0; i < head_weapon_idle.Length; i++)
            {
                head_weapon_idle[i] = null;
            }
            for (int i = 0; i < head_shoot.Length; i++)
            {
                head_shoot[i] = null;
            }
        }
        if (animType == AnimationThpe.body)
        {
            cur_body = null;
            //idle, interact
            for (int i = 0; i < body_idle.Length; i++)
            {
                body_idle[i] = null;
            }
            for (int i = 0; i < body_interact.Length; i++)
            {
                body_interact[i] = null;
            }
            //run
            for (int i = 0; i < body_run_down.Length; i++)
            {
                body_run_down[i] = null;
            }
            for (int i = 0; i < body_run_up.Length; i++)
            {
                body_run_up[i] = null;
            }
            for (int i = 0; i < body_run_right.Length; i++)
            {
                body_run_right[i] = null;
            }
            for (int i = 0; i < body_run_left.Length; i++)
            {
                body_run_left[i] = null;
            }
            //weapon
            for (int i = 0; i < body_weapon_idle.Length; i++)
            {
                body_weapon_idle[i] = null;
            }
            for (int i = 0; i < body_pistol_idle.Length; i++)
            {
                body_pistol_idle[i] = null;
            }
            for (int i = 0; i < body_shoot.Length; i++)
            {
                body_shoot[i] = null;
            }
        }
        if (animType == AnimationThpe.pants)
        {
            cur_pants = null;
            //idle, interact
            for (int i = 0; i < pants_idle.Length; i++)
            {
                pants_idle[i] = null;
            }
            for (int i = 0; i < pants_interact.Length; i++)
            {
                pants_interact[i] = null;
            }
            //run
            for (int i = 0; i < pants_run_down.Length; i++)
            {
                pants_run_down[i] = null;
            }
            for (int i = 0; i < pants_run_up.Length; i++)
            {
                pants_run_up[i] = null;
            }
            for (int i = 0; i < pants_run_right.Length; i++)
            {
                pants_run_right[i] = null;
            }
            for (int i = 0; i < pants_run_left.Length; i++)
            {
                pants_run_left[i] = null;
            }
            //weapon
            for (int i = 0; i < pants_weapon_idle.Length; i++)
            {
                pants_weapon_idle[i] = null;
            }
            for (int i = 0; i < pants_shoot.Length; i++)
            {
                pants_shoot[i] = null;
            }
        }
        if (animType == AnimationThpe.backpack)
        {
            cur_backpack = null;
            //idle, interact
            for (int i = 0; i < backpack_idle.Length; i++)
            {
                backpack_idle[i] = null;
            }
            for (int i = 0; i < backpack_interact.Length; i++)
            {
                backpack_interact[i] = null;
            }
            //run
            for (int i = 0; i < backpack_run_down.Length; i++)
            {
                backpack_run_down[i] = null;
            }
            for (int i = 0; i < backpack_run_up.Length; i++)
            {
                backpack_run_up[i] = null;
            }
            for (int i = 0; i < backpack_run_right.Length; i++)
            {
                backpack_run_right[i] = null;
            }
            for (int i = 0; i < backpack_run_left.Length; i++)
            {
                backpack_run_left[i] = null;
            }
            //weapon
            for (int i = 0; i < backpack_weapon_idle.Length; i++)
            {
                backpack_weapon_idle[i] = null;
            }
            for (int i = 0; i < backpack_shoot.Length; i++)
            {
                backpack_shoot[i] = null;

            }
        }
        if (animType == AnimationThpe.hand)
        {
            cur_hand = null;
            //idle, interact
            for (int i = 0; i < hand_idle.Length; i++)
            {
                hand_idle[i] = null;
            }
            for (int i = 0; i < hand_interact.Length; i++)
            {
                hand_interact[i] = null;
            }
            //run
            for (int i = 0; i < hand_run_down.Length; i++)
            {
                hand_run_down[i] = null;
            }
            for (int i = 0; i < hand_run_up.Length; i++)
            {
                hand_run_up[i] = null;
            }
            for (int i = 0; i < hand_run_right.Length; i++)
            {
                hand_run_right[i] = null;
            }
            for (int i = 0; i < hand_run_left.Length; i++)
            {
                hand_run_left[i] = null;
            }
            //weapon
            for (int i = 0; i < hand_weapon_idle.Length; i++)
            {
                hand_weapon_idle[i] = null;
            }
            for (int i = 0; i < hand_pistol_idle.Length; i++)
            {
                hand_pistol_idle[i] = null;
            }
            for (int i = 0; i < hand_shoot.Length; i++)
            {
                hand_shoot[i] = null;
            }
        }
        if (animType == AnimationThpe.vest)
        {
            cur_vest = null;
            //idle, interact
            for (int i = 0; i < vest_idle.Length; i++)
            {
                vest_idle[i] = null;
            }
            for (int i = 0; i < vest_interact.Length; i++)
            {
                vest_interact[i] = null;
            }
            //run
            for (int i = 0; i < vest_run_down.Length; i++)
            {
                vest_run_down[i] = null;
            }
            for (int i = 0; i < vest_run_up.Length; i++)
            {
                vest_run_up[i] = null;
            }
            for (int i = 0; i < vest_run_right.Length; i++)
            {
                vest_run_right[i] = null;
            }
            for (int i = 0; i < vest_run_left.Length; i++)
            {
                vest_run_left[i] = null;
            }
            //weapon
            for (int i = 0; i < vest_weapon_idle.Length; i++)
            {
                vest_weapon_idle[i] = null;
            }
            for (int i = 0; i < vest_shoot.Length; i++)
            {
                vest_shoot[i] = null;
            }
        }

        NetUpdateMyClotheData();
    }
    public void ClearClothByType(ItemType itemType)
    {
        if (itemType == ItemType.head)
        {
            cur_head = null;
            //idle, interact
            for (int i = 0; i < head_idle.Length; i++)
            {
                head_idle[i] = null;
            }
            for (int i = 0; i < head_interact.Length; i++)
            {
                head_interact[i] = null;
            }
            //run
            for (int i = 0; i < head_run_down.Length; i++)
            {
                head_run_down[i] = null;
            }
            for (int i = 0; i < head_run_up.Length; i++)
            {
                head_run_up[i] = null;
            }
            for (int i = 0; i < head_run_right.Length; i++)
            {
                head_run_right[i] = null;
            }
            for (int i = 0; i < head_run_left.Length; i++)
            {
                head_run_left[i] = null;
            }
            //weapon
            for (int i = 0; i < head_weapon_idle.Length; i++)
            {
                head_weapon_idle[i] = null;
            }
            for (int i = 0; i < head_shoot.Length; i++)
            {
                head_shoot[i] = null;
            }
        }
        if (itemType == ItemType.body)
        {
            cur_body = null;
            //idle, interact
            for (int i = 0; i < body_idle.Length; i++)
            {
                body_idle[i] = null;
            }
            for (int i = 0; i < body_interact.Length; i++)
            {
                body_interact[i] = null;
            }
            //run
            for (int i = 0; i < body_run_down.Length; i++)
            {
                body_run_down[i] = null;
            }
            for (int i = 0; i < body_run_up.Length; i++)
            {
                body_run_up[i] = null;
            }
            for (int i = 0; i < body_run_right.Length; i++)
            {
                body_run_right[i] = null;
            }
            for (int i = 0; i < body_run_left.Length; i++)
            {
                body_run_left[i] = null;
            }
            //weapon
            for (int i = 0; i < body_weapon_idle.Length; i++)
            {
                body_weapon_idle[i] = null;
            }
            for (int i = 0; i < body_pistol_idle.Length; i++)
            {
                body_pistol_idle[i] = null;
            }
            for (int i = 0; i < body_shoot.Length; i++)
            {
                body_shoot[i] = null;
            }
        }
        if (itemType == ItemType.pants)
        {
            cur_pants = null;
            //idle, interact
            for (int i = 0; i < pants_idle.Length; i++)
            {
                pants_idle[i] = null;
            }
            for (int i = 0; i < pants_interact.Length; i++)
            {
                pants_interact[i] = null;
            }
            //run
            for (int i = 0; i < pants_run_down.Length; i++)
            {
                pants_run_down[i] = null;
            }
            for (int i = 0; i < pants_run_up.Length; i++)
            {
                pants_run_up[i] = null;
            }
            for (int i = 0; i < pants_run_right.Length; i++)
            {
                pants_run_right[i] = null;
            }
            for (int i = 0; i < pants_run_left.Length; i++)
            {
                pants_run_left[i] = null;
            }
            //weapon
            for (int i = 0; i < pants_weapon_idle.Length; i++)
            {
                pants_weapon_idle[i] = null;
            }
            for (int i = 0; i < pants_shoot.Length; i++)
            {
                pants_shoot[i] = null;
            }
        }
        if (itemType == ItemType.backpack)
        {
            cur_backpack = null;
            //idle, interact
            for (int i = 0; i < backpack_idle.Length; i++)
            {
                backpack_idle[i] = null;
            }
            for (int i = 0; i < backpack_interact.Length; i++)
            {
                backpack_interact[i] = null;
            }
            //run
            for (int i = 0; i < backpack_run_down.Length; i++)
            {
                backpack_run_down[i] = null;
            }
            for (int i = 0; i < backpack_run_up.Length; i++)
            {
                backpack_run_up[i] = null;
            }
            for (int i = 0; i < backpack_run_right.Length; i++)
            {
                backpack_run_right[i] = null;
            }
            for (int i = 0; i < backpack_run_left.Length; i++)
            {
                backpack_run_left[i] = null;
            }
            //weapon
            for (int i = 0; i < backpack_weapon_idle.Length; i++)
            {
                backpack_weapon_idle[i] = null;
            }
            for (int i = 0; i < backpack_shoot.Length; i++)
            {
                backpack_shoot[i] = null;

            }
        }
        if (itemType == ItemType.shield)
        {
            cur_vest = null;
            //idle, interact
            for (int i = 0; i < vest_idle.Length; i++)
            {
                vest_idle[i] = null;
            }
            for (int i = 0; i < vest_interact.Length; i++)
            {
                vest_interact[i] = null;
            }
            //run
            for (int i = 0; i < vest_run_down.Length; i++)
            {
                vest_run_down[i] = null;
            }
            for (int i = 0; i < vest_run_up.Length; i++)
            {
                vest_run_up[i] = null;
            }
            for (int i = 0; i < vest_run_right.Length; i++)
            {
                vest_run_right[i] = null;
            }
            for (int i = 0; i < vest_run_left.Length; i++)
            {
                vest_run_left[i] = null;
            }
            //weapon
            for (int i = 0; i < vest_weapon_idle.Length; i++)
            {
                vest_weapon_idle[i] = null;
            }
            for (int i = 0; i < vest_shoot.Length; i++)
            {
                vest_shoot[i] = null;
            }
        }
        if (itemType == ItemType.melee || itemType == ItemType.pistol || itemType == ItemType.rifle)
        {
            if(itemType == ItemType.pistol || itemType == ItemType.rifle)
            {
                controllerState = AnimationControllerState.none;
            }
            cur_hand = null;
            //idle, interact
            for (int i = 0; i < hand_idle.Length; i++)
            {
                hand_idle[i] = null;
            }
            for (int i = 0; i < hand_interact.Length; i++)
            {
                hand_interact[i] = null;
            }
            //run
            for (int i = 0; i < hand_run_down.Length; i++)
            {
                hand_run_down[i] = null;
            }
            for (int i = 0; i < hand_run_up.Length; i++)
            {
                hand_run_up[i] = null;
            }
            for (int i = 0; i < hand_run_right.Length; i++)
            {
                hand_run_right[i] = null;
            }
            for (int i = 0; i < hand_run_left.Length; i++)
            {
                hand_run_left[i] = null;
            }
            //weapon
            for (int i = 0; i < hand_weapon_idle.Length; i++)
            {
                hand_weapon_idle[i] = null;
            }
            for (int i = 0; i < hand_pistol_idle.Length; i++)
            {
                hand_pistol_idle[i] = null;
            }
            for (int i = 0; i < hand_shoot.Length; i++)
            {
                hand_shoot[i] = null;
            }
        }
        
        NetUpdateMyClotheData();
    }
    public string GetAnimClothesData()
    {
        string data = $"{cur_bodyskin},{cur_head},{cur_body},{cur_pants},{cur_backpack},{cur_hand},{cur_vest},{controllerState == AnimationControllerState.none}";
        return data;
    }
    public void SetAnimClohesData(string data)
    {
        string[] _data = data.Split(',');
        cur_bodyskin = _data[0];
        cur_head = _data[1];
        cur_body = _data[2];
        cur_pants = _data[3];
        cur_backpack = _data[4];
        cur_hand = _data[5];
        cur_vest = _data[6];
        
        //print("RECIVE DATA: " + data);

        if (cur_bodyskin == "")
        {
            ClearNetClothByType(AnimationThpe.bodyskin);
        }
        else
        {
            SetNetClothByName(cur_bodyskin);
        }
        if (cur_head == "")
        {
            ClearNetClothByType(AnimationThpe.head);
        }
        else
        {
            SetNetClothByName(cur_head);
        }
        if (cur_body == "")
        {
            ClearNetClothByType(AnimationThpe.body);
        }
        else
        {
            SetNetClothByName(cur_body);
        }
        if (cur_pants == "")
        {
            ClearNetClothByType(AnimationThpe.pants);
        }
        else
        {
            SetNetClothByName(cur_pants);
        }
        if (cur_backpack == "")
        {
            ClearNetClothByType(AnimationThpe.backpack);
        }
        else
        {
            SetNetClothByName(cur_backpack);
        }
        if (cur_hand == "")
        {
            ClearNetClothByType(AnimationThpe.hand);
        }
        else
        {
            SetNetClothByName(cur_hand);
        }
        if (cur_vest == "")
        {
            ClearNetClothByType(AnimationThpe.vest);
        }
        else
        {
            SetNetClothByName(cur_vest);
        }

        controllerState = (_data[7] == "true") ? AnimationControllerState.none : AnimationControllerState.rifle;
    }
    public void ResetAll()
    {
        //print("ResetAll animation sprites");
        //BODYSKIN
        controllerState = AnimationControllerState.none;
        for (int i = 0; i < bodyskin_idle.Length; i++)
        {
            bodyskin_idle[i] = null;
        }
        for (int i = 0; i < bodyskin_interact.Length; i++)
        {
            bodyskin_interact[i] = null;
        }
        //run
        for (int i = 0; i < bodyskin_run_down.Length; i++)
        {
            bodyskin_run_down[i] = null;
        }
        for (int i = 0; i < bodyskin_run_up.Length; i++)
        {
            bodyskin_run_up[i] = null;
        }
        for (int i = 0; i < bodyskin_run_right.Length; i++)
        {
            bodyskin_run_right[i] = null;
        }
        for (int i = 0; i < bodyskin_run_left.Length; i++)
        {
            bodyskin_run_left[i] = null;
        }
        //weapon
        for (int i = 0; i < bodyskin_weapon_idle.Length; i++)
        {
            bodyskin_weapon_idle[i] = null;
        }
        for (int i = 0; i < bodyskin_pistol_idle.Length; i++)
        {
            bodyskin_pistol_idle[i] = null;
        }
        for (int i = 0; i < bodyskin_shoot.Length; i++)
        {
            bodyskin_shoot[i] = null;
        }
        //NEXT

        //HEAD
        //idle, interact
        for (int i = 0; i < head_idle.Length; i++)
        {
            head_idle[i] = null;
        }
        for (int i = 0; i < head_interact.Length; i++)
        {
            head_interact[i] = null;
        }
        //run
        for (int i = 0; i < head_run_down.Length; i++)
        {
            head_run_down[i] = null;
        }
        for (int i = 0; i < head_run_up.Length; i++)
        {
            head_run_up[i] = null;
        }
        for (int i = 0; i < head_run_right.Length; i++)
        {
            head_run_right[i] = null;
        }
        for (int i = 0; i < head_run_left.Length; i++)
        {
            head_run_left[i] = null;
        }
        //weapon
        for (int i = 0; i < head_weapon_idle.Length; i++)
        {
            head_weapon_idle[i] = null;
        }
        for (int i = 0; i < head_shoot.Length; i++)
        {
            head_shoot[i] = null;
        }
        //NEXT

        //BODY
        //idle, interact
        for (int i = 0; i < body_idle.Length; i++)
        {
            body_idle[i] = null;
        }
        for (int i = 0; i < body_interact.Length; i++)
        {
            body_interact[i] = null;
        }
        //run
        for (int i = 0; i < body_run_down.Length; i++)
        {
            body_run_down[i] = null;
        }
        for (int i = 0; i < body_run_up.Length; i++)
        {
            body_run_up[i] = null;
        }
        for (int i = 0; i < body_run_right.Length; i++)
        {
            body_run_right[i] = null;
        }
        for (int i = 0; i < body_run_left.Length; i++)
        {
            body_run_left[i] = null;
        }
        //weapon
        for (int i = 0; i < body_weapon_idle.Length; i++)
        {
            body_weapon_idle[i] = null;
        }
        for (int i = 0; i < body_pistol_idle.Length; i++)
        {
            body_pistol_idle[i] = null;
        }
        for (int i = 0; i < body_shoot.Length; i++)
        {
            body_shoot[i] = null;
        }
        //NEXT

        //PANTS
        //idle, interact
        for (int i = 0; i < pants_idle.Length; i++)
        {
            pants_idle[i] = null;
        }
        for (int i = 0; i < pants_interact.Length; i++)
        {
            pants_interact[i] = null;
        }
        //run
        for (int i = 0; i < pants_run_down.Length; i++)
        {
            pants_run_down[i] = null;
        }
        for (int i = 0; i < pants_run_up.Length; i++)
        {
            pants_run_up[i] = null;
        }
        for (int i = 0; i < pants_run_right.Length; i++)
        {
            pants_run_right[i] = null;
        }
        for (int i = 0; i < pants_run_left.Length; i++)
        {
            pants_run_left[i] = null;
        }
        //weapon
        for (int i = 0; i < pants_weapon_idle.Length; i++)
        {
            pants_weapon_idle[i] = null;
        }
        for (int i = 0; i < pants_shoot.Length; i++)
        {
            pants_shoot[i] = null;
        }
        //NEXT

        //BACKPACK
        //idle, interact
        for (int i = 0; i < backpack_idle.Length; i++)
        {
            backpack_idle[i] = null;
        }
        for (int i = 0; i < backpack_interact.Length; i++)
        {
            backpack_interact[i] = null;
        }
        //run
        for (int i = 0; i < backpack_run_down.Length; i++)
        {
            backpack_run_down[i] = null;
        }
        for (int i = 0; i < backpack_run_up.Length; i++)
        {
            backpack_run_up[i] = null;
        }
        for (int i = 0; i < backpack_run_right.Length; i++)
        {
            backpack_run_right[i] = null;
        }
        for (int i = 0; i < backpack_run_left.Length; i++)
        {
            backpack_run_left[i] = null;
        }
        //weapon
        for (int i = 0; i < backpack_weapon_idle.Length; i++)
        {
            backpack_weapon_idle[i] = null;
        }
        for (int i = 0; i < backpack_shoot.Length; i++)
        {
            backpack_shoot[i] = null;

        }
        //NEXT

        //HAND
        //idle, interact
        for (int i = 0; i < hand_idle.Length; i++)
        {
            hand_idle[i] = null;
        }
        for (int i = 0; i < hand_interact.Length; i++)
        {
            hand_interact[i] = null;
        }
        //run
        for (int i = 0; i < hand_run_down.Length; i++)
        {
            hand_run_down[i] = null;
        }
        for (int i = 0; i < hand_run_up.Length; i++)
        {
            hand_run_up[i] = null;
        }
        for (int i = 0; i < hand_run_right.Length; i++)
        {
            hand_run_right[i] = null;
        }
        for (int i = 0; i < hand_run_left.Length; i++)
        {
            hand_run_left[i] = null;
        }
        //weapon
        for (int i = 0; i < hand_weapon_idle.Length; i++)
        {
            hand_weapon_idle[i] = null;
        }
        for (int i = 0; i < hand_pistol_idle.Length; i++)
        {
            hand_pistol_idle[i] = null;
        }
        for (int i = 0; i < hand_shoot.Length; i++)
        {
            hand_shoot[i] = null;

        }
        //NEXT

        //VEST
        //idle, interact
        for (int i = 0; i < vest_idle.Length; i++)
        {
            vest_idle[i] = null;
        }
        for (int i = 0; i < vest_interact.Length; i++)
        {
            vest_interact[i] = null;
        }
        //run
        for (int i = 0; i < vest_run_down.Length; i++)
        {
            vest_run_down[i] = null;
        }
        for (int i = 0; i < vest_run_up.Length; i++)
        {
            vest_run_up[i] = null;
        }
        for (int i = 0; i < vest_run_right.Length; i++)
        {
            vest_run_right[i] = null;
        }
        for (int i = 0; i < vest_run_left.Length; i++)
        {
            vest_run_left[i] = null;
        }
        //weapon
        for (int i = 0; i < vest_weapon_idle.Length; i++)
        {
            vest_weapon_idle[i] = null;
        }
        for (int i = 0; i < vest_shoot.Length; i++)
        {
            vest_shoot[i] = null;
        }

        NetUpdateMyClotheData();
    }

    public void NetUpdateMyClotheData()
    {
        if (playerNetwork.isLocalPlayer)
        {
            playerNetwork.CMDUpdateMyClohes(GetAnimClothesData());
        }
    }
    #endregion
    public void SetActive(bool active)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(active);
        }
    }
}
