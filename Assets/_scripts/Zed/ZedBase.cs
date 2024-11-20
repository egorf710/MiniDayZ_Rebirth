using Assets._scripts.Interfaces;
using Assets._scripts.World;
using Mirror;
using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class ZedBase : NetworkBehaviour, AliveTarget
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private ZedAnimator animator;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private LayerMask raycastLayer;
    [SerializeField] public bool enable;
    [SerializeField] private bool rude;
    [SerializeField] private bool moveTolastPoint;
    [SerializeField] private Transform target;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float stoopingDistance = 0.5f;
    [SerializeField] private float attak—ooldown = 1f;
    [SerializeField] private float searchTime = 1.5f;
    [SerializeField] private float genRandomTime = 0.6f;
    /// <summary>
    /// x - min y - max; in sec
    /// </summary>
    [SerializeField] private Vector2 idleTimeInterval;
    [SerializeField] private float idleDistance;
    [Space]
    [SerializeField] private VulnerabledData vulnerabledData;
    public VulnerableObject myVulController;
    private float nextTime;
    private float idleNextTime;
    private float searchNextTime;
    private float genNextTime;
    private void Start()
    {
        StartCoroutine(IEUpdate());
        StartCoroutine(Check());
        rude = Random.Range(0, 2) == 0;
        UpdateMyState();
    }
    private Vector3 lastTargetPos;
    private float lastDist;
    private Vector3 idleVector = Vector2.zero;
    private float generedDir;
    private IEnumerator IEUpdate()
    {
        while (true)
        {
            if (enable)
            {
                //logic

                RaycastHit2D hit = Physics2D.Raycast(transform.position, target.position - transform.position, 100f, raycastLayer);

                // If it hits something...
                bool itstarget = hit.collider != null && hit.collider.transform == target.transform;
                if (itstarget)
                {
                    if (Vector2.Distance(transform.position, target.position) > stoopingDistance)
                    {
                        Vector2 targetDir = target.transform.position - lastTargetPos;
                        Vector2 moveDir = Vector2.zero;
                        bool b = hit.distance >= lastDist;
                        if (Vector2.Distance(transform.position, target.position) < 2 || !b)
                        {
                            moveDir = (target.position - transform.position);
                        }
                        else
                        {
                            moveDir = targetDir;
                        }

                        rb.velocity = moveDir.normalized * speed * Time.fixedDeltaTime;
                        
                        searchNextTime = Time.time + searchTime;
                        animator.AnimMove(rb.velocity);
                    }
                    else
                    {
                        Attak(hit.collider.transform);
                        yield return new WaitForSeconds(0.1f);
                    }
                    lastTargetPos = target.position;
                    lastDist = hit.distance - 0.1f;
                    moveTolastPoint = true;
                }
                else if(Time.time < searchNextTime && !itstarget && target != null)
                {

                    bool moveY = Mathf.Abs(hit.normal.x) > Mathf.Abs(hit.normal.y);
                    Vector2 moveDir = Vector2.zero;

                    if (Time.time > genNextTime && !moveTolastPoint)
                    {
                        genNextTime = Time.time + genRandomTime;
                        generedDir = Random.Range(0, 1f);
                    }
                    if (moveTolastPoint)
                    {
                        moveDir = lastTargetPos - transform.position;

                        if(Vector2.Distance(lastTargetPos, transform.position) < stoopingDistance)
                        {
                            moveTolastPoint = false;
                        }
                    }
                    else
                    {
                        moveDir = new Vector2(moveY ? 0 : (generedDir > 0.5f ? 1 : -1), moveY ? (generedDir > 0.5f ? 1 : -1) : 0);

                        if (hit.distance > stoopingDistance)
                        {
                            moveDir = (target.position - transform.position);
                        }
                        else
                        {
                            moveDir = -moveDir;
                        }
                    }
                    rb.velocity = moveDir.normalized * speed * Time.fixedDeltaTime;
                    
                    animator.AnimMove(rb.velocity);
                }
                else // idle
                {

                    if (Time.time >= idleNextTime)
                    {
                        idleNextTime = Time.time + Random.Range(idleTimeInterval.x, idleTimeInterval.y);

                        idleVector = transform.position + new Vector3(Random.Range(-idleDistance, idleDistance), Random.Range(-idleDistance, idleDistance), 0);

                        RaycastHit2D hitIdle = Physics2D.Raycast(transform.position, idleVector - transform.position, 100f, raycastLayer);
                        if(hitIdle.collider != null)
                        {
                            idleVector = hitIdle.point;
                        }
                    }

                    if(Vector2.Distance(idleVector, transform.position) > stoopingDistance)
                    {
                        rb.velocity = (idleVector - transform.position).normalized * speed / 2 * Time.fixedDeltaTime;
                        animator.AnimMove(rb.velocity, true);
                    }
                    else
                    {
                        rb.velocity = Vector2.zero;
                        animator.AnimIdle();
                    }

                    moveTolastPoint = false;
                }

                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                yield return new WaitForSeconds(1f);
            }
        }
    }
    private IEnumerator Check()
    {
        while (true)
        {
            if (!enable)
            {
                yield return new WaitForSeconds(3f);
            }
            else
            {
                yield return new WaitForSeconds(1f);
            }
            UpdateMyState();
        }
    }
    private void UpdateMyState()
    {
        Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position, 20, targetLayer);
        if (colls.Length > 0)
        {
            enable = true;
            target = colls[0].transform;
            rb.simulated = true;
            animator.enabled = true;
            TargetManager.AddTarget(this);
        }
        else
        {
            enable = false;
            target = null;
            rb.simulated = false;
            animator.enabled = false;
            TargetManager.RemoveTarget(this);
        }
    }
    private void Attak(Transform vulTarget)
    {
        if(Time.time >= nextTime)
        {
            nextTime = Time.time + attak—ooldown;
            rb.velocity = Vector2.zero;
            print("attack vul: " + vulTarget.name);
        }
    }

    public void TakeDamage(int damage, int code = 0)
    {
        int attackChanse = Random.Range(0, 100);
        if(attackChanse <= vulnerabledData.blockChanse)
        {
            return;
        }
        vulnerabledData.health -= Mathf.Clamp(damage - vulnerabledData.armor, 
            1, damage);
        DebuMessager.Mess("-" + damage, Color.red, transform.position, true);
        if(vulnerabledData.health <= 0)
        {
            StopAllCoroutines();
            speed = 0;
            enable = false;
            target = null;
            rb.simulated = false;
            TargetManager.RemoveTarget(this);


            myVulController.DropLoot();
            animator.AnimDie();
            enabled = false;
        }
    }

    public Transform getTransform()
    {
        return transform;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        TargetManager.RemoveTarget(this);
    }

    public uint getNetID()
    {
        return netId;
    }
}
