using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum AnimationThpe { bodyskin, head, body, pants, backpack, hand, vest }
[CreateAssetMenu(menuName = "AnimationItem", fileName = "new AnimationItem")]
public class AnimationItem : ScriptableObject
{
    public AnimationThpe type;
    [Header("Idle")]
    public Sprite[] idle = new Sprite[4];
    [Header("Run")]
    public Sprite[] run_down = new Sprite[4];
    public Sprite[] run_up = new Sprite[4];
    public Sprite[] run_right = new Sprite[4];
    public Sprite[] run_left = new Sprite[4];
    [Header("Interact")]
    public Sprite[] interact = new Sprite[4];
    [Header("Shoot Idle")]
    public Sprite[] shoot = new Sprite[4];
    [Header("Shoot aim")]
    public Sprite[] weapon_idle = new Sprite[4];
    public Sprite[] pistol_idle = new Sprite[4];
}
