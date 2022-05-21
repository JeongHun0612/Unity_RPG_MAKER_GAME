using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MovingObject
{
    static public PlayerManager instance;

    public string currentMapName;

    public string walkSound_1;
    public string walkSound_2;
    public string walkSound_3;
    public string walkSound_4;

    private AudioManager theAudio;

    public float runSpeed;
    private float applyRunSpeed;
    private bool applyRunFlag = false;
    private bool canMove = true;

    public bool notMove = false;

    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        queue = new Queue<string>();
        boxCollider = GetComponent<BoxCollider2D>();
        theAudio = FindObjectOfType<AudioManager>();
        animator = GetComponent<Animator>();
    }

    IEnumerator MoveCoroutine()
    {
        while (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0 && !notMove)
        {
            // 빠른 이동 키 입력시
            if (Input.GetKey(KeyCode.LeftShift))
            {
                applyRunSpeed = runSpeed;
                applyRunFlag = true;
            }
            else
            {
                applyRunSpeed = 0;
                applyRunFlag = false;
            }

            // 이동키 입력시
            vector.Set(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), transform.position.z);

            // 동시 키 입력시 애니메이션 겹침 방지
            if (vector.x != 0)
                vector.y = 0;

            // 애니메이터 값 저장
            animator.SetFloat("DirX", vector.x);
            animator.SetFloat("DirY", vector.y);

            // 움직이는 방향에 충돌체가 있으면 정지
            bool checkCollsionFlag = base.CheckCollsion();
            if (checkCollsionFlag)
                break;

            animator.SetBool("Walking", true);

            // 4가지의 WalkSound 중 랜덤 1종 실행
            int temp = Random.Range(1, 4);
            switch (temp)
            {
                case 1:
                    theAudio.Play(walkSound_1);
                    break;
                case 2:
                    theAudio.Play(walkSound_2);
                    break;
                case 3:
                    theAudio.Play(walkSound_3);
                    break;
                case 4:
                    theAudio.Play(walkSound_4);
                    break;
            }

            // BoxCollider를 움직일 방향으로 미리 이동
            boxCollider.offset = new Vector2(vector.x * 0.7f * speed * walkCount, vector.y * 0.7f * speed * walkCount);

            // 실제 위치 값 변경 (이동)
            while (currentWalkCount < walkCount)
            {
                transform.Translate(vector.x * (speed + applyRunSpeed), vector.y * (speed + applyRunSpeed), 0);

                // 빠른 이동 조건 시
                if (applyRunFlag)
                    currentWalkCount++;
                currentWalkCount++;

                // BoxCollider 위치 값 초기화
                if (currentWalkCount == 12)
                    boxCollider.offset = Vector2.zero;

                yield return new WaitForSeconds(0.01f);
            }
            currentWalkCount = 0;
        }
        animator.SetBool("Walking", false);
        canMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove && !notMove)
        {
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                canMove = false;
                StartCoroutine(MoveCoroutine()); // 코루틴 실행
            }
        }
    }
}
