using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;

    public int maxHealth = 5;

    public GameObject projectilePrefab;

    public float timeInvincible = 2.0f;

    public int health { get { return currentHealth; }}
    int currentHealth;

    bool isInvincible;
    float invincibleTimer;

    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;

    Animator animator;
    Vector2 lookDirection = new Vector2(1,0);

    public AudioSource audioSource;

    public AudioClip throwSound;
    public AudioClip hitSound;

    public ParticleSystem healthDecrease;
    public ParticleSystem healthIncrease;

    int score = 0;
    public TextMeshProUGUI scoreText;

    public GameObject winText;
    public GameObject loseText;
    public GameObject gameOverText;

    bool gameOver;

    public AudioClip winMusic;
    public AudioClip loseMusic;
    public AudioClip backgroundMusic;

    public static int level;

    public int cogs = 5;
    public TextMeshProUGUI cogsText;


    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        animator = GetComponent<Animator>();

        audioSource = GetComponent<AudioSource>();

        SetScoreText();
        
        winText.SetActive(false);
        loseText.SetActive(false);
        gameOverText.SetActive(false);
        gameOver = false;

        audioSource.clip = backgroundMusic;
        audioSource.loop = true;
        audioSource.Play();

        cogs = 5;
        SetCogsText();

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
            if (cogs >= 1)
            {
                Launch();
                cogs -= 1;
                SetCogsText();
            }
            
        }

        if(Input.GetKey(KeyCode.R))
        {
            if (gameOver == true)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));

            if (hit.collider != null)
            {
                if (score == 5)
                {
                    SceneManager.LoadScene("Scene 2");
                    level = 2;
                    score = 0;

                    audioSource.clip = backgroundMusic;
                    audioSource.loop = true;
                    audioSource.Play();
                }
            
                else
                {
                    NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                    if (character != null)
                    {
                        character.DisplayDialog();
                    }
                }
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
            animator.SetTrigger("Hit");
            if (isInvincible)
                return;

            isInvincible = true;
            invincibleTimer = timeInvincible;
            ParticleSystem healthParticle = Instantiate(healthDecrease, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
            PlaySound(hitSound);
        }
        if (amount > 0)
        {
            ParticleSystem healthParticle = Instantiate(healthIncrease, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);

        if (currentHealth == 0)
        {
            loseText.SetActive(true);
            gameOver = true;
            speed = 0;
            isInvincible = true;

            audioSource.Stop();
            PlaySound(loseMusic);
        }

    }

    void SetScoreText()
    {
        scoreText.text = "Robots Fixed: " + score.ToString() + "/5";

        if(score == 5)
        {
            gameOverText.SetActive(true);
            {
            if (level == 2)
                {
                    if (score == 5)
                    {
                        winText.SetActive(true);
                        gameOver = true;
                        speed = 0;

                        audioSource.Stop();
                        PlaySound(winMusic);
                    }
                }
            }
        }
    }

    public void ChangeScore(int amount)
    {
        score += amount;
        SetScoreText();
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

    public void SetCogsText()
    {
        cogsText.text = "Cogs: " + cogs.ToString();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "CollectCog")
        {
            cogs += 4;
            cogsText.text = "Cogs: " + cogs.ToString();
            Destroy(collision.collider.gameObject);
        }
    }

}
