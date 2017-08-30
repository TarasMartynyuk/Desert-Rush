using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Vehicle : Actor 
{
    [SerializeField] GameObject[] pathCheckpoints;
    [SerializeField] Image healthBar, fuelBar;
    
    //stats
    
    float maxFuel = 200;
    float fuel = 15;
    Animator anim;

    //
    Rigidbody2D rgb;
    int currentCheckpointIndex;
    Vector3 currentCheckpoint;
    
    Vector3 newPos;
    
    //fuel system support
    float distanceTravelled;
    int nextIntegerDistance = 1;
    Vector2 positionLastFrame;

    //turn support
    bool turning;
    float turnSpeed = 8f;

    //music
    AudioSource audioS;
    [SerializeField] AudioClip engineDIeSFX;

    float maxHealth;
    bool moving;

    //shield support
   [SerializeField]  Shield shield;

    

#region Unity funcs  
    void Awake()
    {   
        audioS = GetComponent<AudioSource>();

        currentCheckpointIndex = 0;
        currentCheckpoint = pathCheckpoints[currentCheckpointIndex].transform.position;

        rgb = GetComponent<Rigidbody2D>();
        positionLastFrame = rgb.position;

        maxHealth = health;
        updateFuelBar();

        anim = GetComponent<Animator>();
        if(anim == null)
        {
        }
    }
    
    // void Update()
    // {
        
        
    // }

  
    void FixedUpdate()
    {
        if(GameController.Instance.CurrState != GameState.GAME)
			return;

        if(turning)
            return;

        if(fuel >= 0)
        {
            if(moving == false)
            {
                audioS.Play();
                moving = true;
            }

            // if(anim.pla)

            //move towards curr checkpoint
            newPos = Vector3.MoveTowards(transform.position, currentCheckpoint, speed * Time.deltaTime);
            rgb.MovePosition(newPos);

            //calculate distance travelled this frame
            distanceTravelled += Vector2.Distance(rgb.position, positionLastFrame);

            //if we travelled another unit, substract 1 fuel
            if(distanceTravelled >= nextIntegerDistance)
            {
                fuel--;
                nextIntegerDistance++;
                updateFuelBar();
            }

            positionLastFrame = rgb.position;
        }
        else
        {
            if(moving)
            {
                audioS.Stop();
                audioS.PlayOneShot(engineDIeSFX);
                moving = false;
            }
        }
    }
	
    
    void OnTriggerEnter2D(Collider2D other)
    {

        if(other.tag == "Path")
        {
            currentCheckpointIndex ++;
            if(currentCheckpointIndex >= pathCheckpoints.Length)
            {
                //win
                GameController.Instance.TransferToEndGameState(true, false);
            }
            currentCheckpoint = pathCheckpoints[currentCheckpointIndex].transform.position;

            TurnToFaceCurrentCheckpoint();
        }
        else if(other.tag == "StateChanger")
        {
            SpawnController.Instance.AdvanceDifficultyLevel();
            Destroy(other.gameObject);
        }

    }
    
#endregion

    public override void SufferDamage(int damage)
    {
        health -= damage;
        updateHealthBar();

        if(health <= 0 )
        {
            //game over
            GameController.Instance.TransferToEndGameState(false, false);

        }

        shield = GetComponent<Shield>();
    }

    public void ReplenishFuel(int fuelSupply)
    {
        this.fuel += fuelSupply;

        if(fuel > maxFuel)
            fuel = (int)maxFuel;

        updateHealthBar();
    }

    public bool TryActivateShield()
    {
        return shield.TryActivate();
    }

    void TurnToFaceCurrentCheckpoint()
    {
        StartCoroutine(TurnToFaceTargetCouroutine(currentCheckpoint, turnSpeed));
    }

    IEnumerator TurnToFaceTargetCouroutine(Vector3 target, float turnSpeed)
    {
        turning = true;

        Vector3 vectorToTarget = target - transform.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        // Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        Quaternion q = Quaternion.LookRotation(Vector3.forward, vectorToTarget);

        while(transform.rotation != q)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * turnSpeed);
            yield return GameController.Instance.WaitForEndOfFrameSync();
        }
        
        turning = false;
    }

    void updateHealthBar()
    {
        healthBar.fillAmount = (float) health / maxHealth;
    }

    void updateFuelBar()
    {
        fuelBar.fillAmount = (float)fuel / maxFuel;
    }
}
