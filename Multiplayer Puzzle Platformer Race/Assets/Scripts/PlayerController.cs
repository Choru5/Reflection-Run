using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPunCallbacks
{

    public PhotonView pv;

    private float moveHorizontal;
    private float moveVertical;
    private Vector2 currentVelocity;
    [SerializeField]
    private float movementSpeed = 3f;
    private Rigidbody2D characterRigidBody;
    public bool isJumping;
    [SerializeField]
    private float jumpForce = 100f;
    private bool alreadyJumped = false;

    public SpriteRenderer spr;
    public Sprite[] s;

    int currentFrame;
    int currentSprite;
    bool flipped;

    public bool inGame = false;

    public bool isBlack = false;

    public GameObject cam;

    public Transform blackCam;
    public Transform whiteCam;

    public TMP_Text displayNames;

    public GameObject waitingText;

    public bool started = false;

    public string username;

    public AudioSource jumpAudio;
    public AudioSource hitGround;

    // Start is called before the first frame update
    void Start()
    {
        jumpAudio = GameObject.FindGameObjectWithTag("AudioJump").GetComponent<AudioSource>();
        hitGround = GameObject.FindGameObjectWithTag("AudioHit").GetComponent<AudioSource>();
        username = PhotonNetwork.LocalPlayer.NickName;
        started = false;
        inGame = false;
        waitingText = GameObject.FindGameObjectWithTag("Waiting");
        cam = FindObjectOfType<Camera>().gameObject;        
        this.characterRigidBody = GetComponent<Rigidbody2D>();
        pv = GetComponent<PhotonView>();
        Invoke("HandleAnimation", 0.25f);
        blackCam = GameObject.FindGameObjectWithTag("CamBlack").transform;
        whiteCam = GameObject.FindGameObjectWithTag("CamWhite").transform;

        displayNames = GameObject.FindGameObjectWithTag("DisplayName").GetComponent<TMP_Text>();

        if (pv.IsMine)
        {
            gameObject.tag = "Player";

            if (isBlack)
            {
                cam.transform.position = blackCam.position;
                cam.transform.rotation = blackCam.rotation;
            }

            else if (!isBlack)
            {
                cam.transform.position = whiteCam.position;
                cam.transform.rotation = whiteCam.rotation;
            }
        }

        else
        {
            gameObject.tag = "Untagged";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!started && inGame)
        {
            waitingText.SetActive(false);
            started = true;
        }

        if (pv.IsMine && inGame)
        {
            this.moveHorizontal = Input.GetAxis("Horizontal");
            this.moveVertical = Input.GetAxis("Vertical");
            this.currentVelocity = this.characterRigidBody.velocity;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!isJumping)
                {
                    isJumping = true;
                    alreadyJumped = false;

                    jumpAudio.Play();
                    StartCoroutine("stopSounds");
                }
            }
        }

        spr.sprite = s[currentSprite];
        spr.flipX = flipped;
    }

    private void FixedUpdate()
    {
        if (pv.IsMine && inGame)
        {
            if (this.moveHorizontal != 0)
            {
                this.characterRigidBody.velocity = new Vector2(this.moveHorizontal * this.movementSpeed, this.currentVelocity.y);

                pv.RPC("ChangeAnim", RpcTarget.All, currentFrame);
            }
            else
            {
                pv.RPC("ChangeAnim", RpcTarget.All, 2);
            }

            if (isBlack)
            {
                if (this.moveHorizontal > 0)
                    pv.RPC("ChangeFlipState", RpcTarget.All, false);
                else if (this.moveHorizontal < 0)
                    pv.RPC("ChangeFlipState", RpcTarget.All, true);
            }

            else if (!isBlack)
            {
                if (this.moveHorizontal > 0)
                    pv.RPC("ChangeFlipState", RpcTarget.All, true);
                else if (this.moveHorizontal < 0)
                    pv.RPC("ChangeFlipState", RpcTarget.All, false);
            }

            if (isJumping && !alreadyJumped)
            {
                this.characterRigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Force);
                this.alreadyJumped = true;
            }
        }
    }

    [PunRPC]
    public void ChangeAnim(int _ID)
    {
        currentSprite = _ID;
    }

    [PunRPC]
    public void ChangeFlipState(bool _flipState)
    {
        flipped = _flipState;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            this.isJumping = false;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            this.isJumping = false;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            this.isJumping = true;
        }
    }

    void HandleAnimation()
    {
        if (currentFrame == 0)
            currentFrame = 1;
        else if (currentFrame == 1)
            currentFrame = 0;

        Invoke("HandleAnimation", 0.25f);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        pv.RPC("SyncNameDisplay", RpcTarget.All, "" + newPlayer.NickName + " vs " + PhotonNetwork.LocalPlayer.NickName);
        Debug.Log("Name = " + newPlayer.NickName);
        base.OnPlayerEnteredRoom(newPlayer);
    }

    [PunRPC]
    public void SyncNameDisplay(string _nameDisplay)
    {
        Debug.Log("Names Synced");
        displayNames.text = _nameDisplay;
    }

    public IEnumerator stopSounds()
    {
        hitGround.mute = true;
        yield return new WaitForSeconds(0.2f);
        hitGround.mute = false;
    }
}
