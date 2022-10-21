using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerScript : MonoBehaviour
{
    private Rigidbody2D rd2d;
    public float speed;
    public float jump;

    // Animator
    public Animator animator;
    private bool facingRight = true;

    // Score and Lives Text and Integers
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI livesText;
    private int scoreValue;
    private int livesValue;

    // Win and Lose Text
    public GameObject WinTextObject;
    public GameObject LoseTextObject;

    // Ground Check Variables
    private bool isOnGround;
    public Transform groundcheck;
    public float checkRadius;
    public LayerMask allGround;

    // Start is called before the first frame update
    void Start()
    {
        rd2d = GetComponent<Rigidbody2D>();
        scoreValue = 0;

        rd2d = GetComponent<Rigidbody2D>();
        livesValue = 3;

        SetCountText();
        WinTextObject.SetActive(false);

        SetCountText();
        LoseTextObject.SetActive(false);
    }

    void SetCountText()
    {
        scoreText.text = "Score: " + scoreValue.ToString();
        if (scoreValue >= 8)
        {
            WinTextObject.SetActive(true);
            Destroy(gameObject);

            // Calls sound script and plays win sound
            SoundMangerScript.PlaySound("FFWin");
        }

        // Lives reset and Teleport to next level - (BUGGED RIGHT NOW) - Has to do with OnCollisionEnter2D function and SetCountText(); maybe??
        scoreText.text = "Score: " + scoreValue.ToString();
        if (scoreValue == 4) // How can I make this function only perform ONCE?? Cause when score equals 5, this function doesn't run. But as long as its 4, and a player collides with an enemy, it doesn't reduce lives and it teleports player again.
        {
            livesValue = 3;
            transform.position = new Vector2(100f, 0.5f);
        }

        livesText.text = "Lives: " + livesValue.ToString();
        if (livesValue == 0)
        {
            LoseTextObject.SetActive(true);
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float hozMovement = Input.GetAxis("Horizontal");
        float vertMovement = Input.GetAxis("Vertical");
        rd2d.AddForce(new Vector2(hozMovement * speed, vertMovement * speed));

        // Ground Check
        isOnGround = Physics2D.OverlapCircle(groundcheck.position, checkRadius, allGround);

        // Animation
        animator.SetFloat("HorizontalValue", Mathf.Abs(Input.GetAxis("Horizontal")));
        animator.SetFloat("VerticalValue", Input.GetAxis("Vertical"));

        if (facingRight == false && hozMovement > 0)
        {
            Flip();
        }

        else if (facingRight == true && hozMovement < 0)
        {
            Flip();
        }
    }

    // This Function Increases Score Value
    private void OnCollisionEnter2D(Collision2D collision)
    {
       if (collision.collider.tag == "Coin")
        {
            scoreValue += 1;
            SetCountText();
            Destroy(collision.collider.gameObject);
        }

        // This Function Decreases Lives Value
        if (collision.collider.tag == "Enemy") //this line is different compared to other tutorial because function is a collider
        {
            Destroy(collision.collider.gameObject);
            livesValue = livesValue - 1;

            SetCountText();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // Jump code
        if (collision.collider.tag == "Ground" && isOnGround)
        {
            if (Input.GetKey(KeyCode.W))
            {
                rd2d.AddForce(new Vector2(0f, jump), ForceMode2D.Impulse);
            }
        }
    }

    // Close Application
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector2 Scaler = transform.localScale;
        Scaler.x = Scaler.x * -1;
        transform.localScale = Scaler;
    }
}
