using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    public float health = 100;
    public AudioClip[] bodyshotClips;
    public AudioClip[] headshotClips;

    NavMeshAgent agent;
    Animator anim;
    AudioSource audio;

    Transform player;
    Vector3 Target;

    float attackTimer;

    bool moveStop;
    bool dontTriggerTwice = true;
    bool chasing;
    bool attacking;
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
        player = FindObjectOfType<Player>().transform;

        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
            rb.isKinematic = true;

        agent.speed = 0.5f;
        Target = transform.position;
        
    }

    void Update()
    {
        agent.destination = Target;
        if (Vector3.Distance(transform.position, Target) <= 0.5f)
            anim.SetBool("Walk", false);
        else
            anim.SetBool("Walk", true);

        if (health > 0)
        {
            if (moveStop == false)
            {
                if (chasing == false)
                {//沒在追
                    Target = transform.position;
                    if (Vector3.Distance(transform.position, player.position) <= 10f && Mathf.Abs(transform.position.y - player.position.y) <= 2f)
                        chasing = true;//太近就追了
                }
                else
                {//有在追
                    Target = player.position;
                    if (Vector3.Distance(transform.position, player.position) >= 30f || Mathf.Abs(transform.position.y - player.position.y) >= 10f)
                        chasing = false;//太遠就不追了
                    if (Vector3.Distance(transform.position, player.position) <= 2f)
                        attacking = true;//距離到就攻擊
                    else
                        attacking = false;//距離不到繼續追
                }
            }

            attackTimer += Time.deltaTime;

            if (attacking == true)
            {//有在打
                Target = transform.position;
                if (attackTimer >= 2f) //每次攻擊間隔兩秒
                {
                    transform.forward = new Vector3(player.position.x, 0, player.position.z) - new Vector3(transform.position.x, 0, transform.position.z);
                    if (dontTriggerTwice)
                    {
                        dontTriggerTwice = false;
                        anim.SetTrigger("Attack");
                    }
                }
            }
        }
    }
    public void AttackAnimationBegin()
    {
        moveStop = true;
    }

    public void AttackAnimationDamage()
    {
        if (Vector3.Distance(transform.position, player.position) <= 2.5f) //如果攻擊判定還在範圍內則受到攻擊
        {
            int damage = 25 + Random.Range(-10, 10);
            player.GetComponent<PlayerData>().hp -= damage;
            print(damage);
        }
    }
    public void AttackAnimationEnd()
    {
        moveStop = false;
        dontTriggerTwice = true;
        attackTimer = 0;
    }

    public void TakeDamage(bool isHeadShot, float damage)
    {
        if (!isHeadShot)
        {
            health -= damage;
            audio.clip = bodyshotClips[Random.Range(0, bodyshotClips.Length)];
            audio.Play();
        }
        else
        {
            health -= damage * 3f;
            audio.clip = headshotClips[Random.Range(0, headshotClips.Length)];
            audio.Play();
        }

        if (health <= 0)
        {
            agent.destination = transform.position;
            agent.enabled = false;
            
            GetComponent<Animator>().enabled = false;
            foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
                rb.isKinematic = false;

            foreach (Collider col in GetComponentsInChildren<Collider>())
            {
                Physics.IgnoreCollision(col, FindObjectOfType<Player>().GetComponent<Collider>(), true);
            }

            this.enabled = false;
        }
    }
}
