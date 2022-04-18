using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;
    public int maxHealth = 5;
    private int scoreValue = 0;
    public int cogs = 5;
    public int spells = 0;

    bool gameOver;

    public Text fixedText;
    public Text spellText;
    public GameObject winTextObject;
    public GameObject nextLevelTextObject;
    public GameObject loseTextObject;
    public Text cogsText;
    
    public GameObject projectilePrefab;
    public GameObject damagePrefab;
    public GameObject healthPrefab;
    
    public AudioClip throwSound;
    public AudioClip hitSound;

    public AudioClip winSound; //Win
    public AudioClip loseSound; //Lose
    public AudioSource musicSource;
    
    public int health { get { return currentHealth; }}

    int currentHealth;
    
    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;
    
    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;
    
    Animator animator;
    Vector2 lookDirection = new Vector2(1,0);
    
    AudioSource audioSource;

    public static int level = 1;
    public static int damageTaken = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
        currentHealth = maxHealth;

        winTextObject.SetActive(false);
        loseTextObject.SetActive(false);
        nextLevelTextObject.SetActive(false);

        audioSource = GetComponent<AudioSource>();

        fixedText.text = "Fixed Robots: " + scoreValue.ToString();
        cogsText.text = "Cogs: " + cogs.ToString();

        musicSource.Play();

        gameOver = false;
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        
        Vector2 move = new Vector2(horizontal, vertical);
        
        if(!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }
        
        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);
        
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }
        
        if(Input.GetKeyDown(KeyCode.C))
        {
            if(cogs >= 1)
            {
            Launch();
            cogs -= 1;
            cogsText.text = "Cogs: " + cogs.ToString();
            }
        }
        
        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                if(scoreValue < 5 && level == 1)
                {
                    NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                    if (character != null)
                    {
                        character.DisplayDialog();
                    }
                }

                else if(scoreValue < 5 && level == 2)
                {
                    NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                    if (character != null)
                    {
                        character.DisplayDialog();
                    }
                }

                else
                {
                    NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                    if (character.tag == "NPC" && scoreValue >= 5)
                    {
                    if (character != null)
                    {
                         SceneManager.LoadScene("Main2");
                         level = 2;
                         spellText.text = "Spells: " + spells.ToString();
                    }
                    }
                }
            }
            Debug.Log("The static level is at " + level);
        }

        if (scoreValue >= 5 && level == 2 && spells == 4)
        {
            // Set the text value of your 'winText'
            winTextObject.SetActive(true);
            speed = 0.0f;
            gameOver = true;
            musicSource.Stop();

            //Play victory sound
            PlaySound(winSound);
        }

        if (scoreValue >= 5 && level == 1)
        {
            // Set the text value of your 'winText'
            gameOver = false;
            nextLevelTextObject.SetActive(true);
            Destroy(nextLevelTextObject, 3.0f);  

            PlaySound(winSound); 
        }

        if (currentHealth <= 0)
        {
            // Set the text value of your 'winText'
            loseTextObject.SetActive(true);
            speed = 0.0f;
            gameOver = true;
            musicSource.Stop();

            //Play lose sound
            PlaySound(loseSound);
        }

         if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("The gameOver boolean is " + gameOver);
           if (gameOver == true && scoreValue < 5)
            {
              SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // this loads the currently active scene
            }
            else if (gameOver == true && scoreValue >= 5)
            {
                level = 1;
                SceneManager.LoadScene("Main1");
            }
        }
    }
    
    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
                return;
            
            isInvincible = true;
            invincibleTimer = timeInvincible;
             //this might be a good place to instantiate your "damage" particles
             GameObject damageParticle = Instantiate(damagePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
            
            animator.SetTrigger("Hit");
            PlaySound(hitSound);
            damageTaken += 1;
            Debug.Log("Damage Taken is " + damageTaken);
        }
        if (amount > 0)
        {
             GameObject healthParticle = Instantiate(healthPrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        }
        
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
    }

    public void ChangeScore()
    { 
        scoreValue += 1;
        fixedText.text = "Fixed Robots: " + scoreValue.ToString();
    }

    public void ChangeAmmo()
    { 
        cogs += 4;
        cogsText.text = "Cogs: " + cogs.ToString();
    }
    
    public void ChangeSpell()
    { 
        spells += 1;
        spellText.text = "Spells: " + spells.ToString();
    }

    public void ChangeSpeed()
    { 
        speed = 5.0f;
    }

    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");
        
        PlaySound(throwSound);
    } 
    
    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}