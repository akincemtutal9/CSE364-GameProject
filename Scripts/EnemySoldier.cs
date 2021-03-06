using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySoldier : MonoBehaviour
{
    public float enemyHealth;
    float currentHealth;
    public bool isDead;
    public GameObject projectile;
    public Transform projectilePoint;
    public Slider healthBar;
    
    public Animator anim;
    public CapsuleCollider cc;
    private void Start()
    {
        currentHealth = enemyHealth;
        anim = GetComponent<Animator>();
        cc = GetComponent<CapsuleCollider>();
        healthBar.value = enemyHealth;
    }
    private void Update()
    {
        //healthBar.value = currentHealth;
    }
    public void Shoot()
    {
      Rigidbody rb = Instantiate(projectile, projectilePoint.position , Quaternion.identity).GetComponent<Rigidbody>();
      rb.AddForce(transform.forward * 30f, ForceMode.Impulse);
      rb.AddForce(transform.up * 7f, ForceMode.Impulse);  
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.value -= damage;
        anim.SetTrigger("damage");
        if (currentHealth <= 0)
        {
            anim.SetTrigger("death");
            Die();
        }
    }
    public void Die() 
    {
        Debug.Log("Enemy died");
        //disable enemy
        anim.SetTrigger("death");
        isDead = true;
        GetComponent<CapsuleCollider>().enabled = false;
        this.enabled = false;
        StartCoroutine(DieAfterFiveSeconds());
    }

    private IEnumerator DieAfterFiveSeconds()
    {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);


    }

}
