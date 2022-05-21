using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    static public DialogueManager instance;

    #region Singleton
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
    #endregion Singleton

    public Text text;
    public SpriteRenderer rendererSprite;
    public SpriteRenderer rendererDialogueWindow;

    private List<string> listSentences;
    private List<Sprite> listSprites;
    private List<Sprite> listDialogueWindows;

    private int count; // 대화 진행 상황 카운트

    public Animator animSprites;
    public Animator animDialogueWindow;

    public string typeSound;
    public string enterSound;

    private AudioManager theAudio;
    private OrderManager theOrder;

    public bool isTalking = false;
    private bool keyActivated = false;

    // Start is called before the first frame update
    void Start()
    {
        count = 0;
        text.text = "";
        listSentences = new List<string>();
        listSprites = new List<Sprite>();
        listDialogueWindows = new List<Sprite>();
        theAudio = FindObjectOfType<AudioManager>();
        theOrder = FindObjectOfType<OrderManager>();
    }

    public void ShowDialogue(Dialogue dialogue)
    {
        // 대화 시작 boolean 변수
        isTalking = true;

        theOrder.NotMove();

        // List에 값 추가
        for (int i = 0; i < dialogue.sentences.Length; i++)
        {
            listSentences.Add(dialogue.sentences[i]);
            listSprites.Add(dialogue.sprites[i]);
            listDialogueWindows.Add(dialogue.dialogueWindows[i]);
        }

        // 애니메이션 값 설정 (true = 대화창 출력)
        animSprites.SetBool("Appear", true);
        animDialogueWindow.SetBool("Appear", true);

        StartCoroutine(StartDialogueCoroutine());
    }

    public void ExitDialogue()
    {
        count = 0;
        text.text = "";
        listSentences.Clear();
        listSprites.Clear();
        listDialogueWindows.Clear();
        animSprites.SetBool("Appear", false);
        animDialogueWindow.SetBool("Appear", false);
        isTalking = false;
        theOrder.Move();
    }

    IEnumerator StartDialogueCoroutine()
    {
        if (count > 0)
        {
            // DialogueWindows가 이전 DialogueWindows와 같지않으면 DialogueWindows와 Sprite를 변경
            if (listDialogueWindows[count] != listDialogueWindows[count - 1])
            {
                animSprites.SetBool("Change", true);
                animDialogueWindow.SetBool("Appear", false);
                yield return new WaitForSeconds(0.2f);
                rendererDialogueWindow.GetComponent<SpriteRenderer>().sprite = listDialogueWindows[count];
                rendererSprite.GetComponent<SpriteRenderer>().sprite = listSprites[count];
                animDialogueWindow.SetBool("Appear", true);
                animSprites.SetBool("Change", false);
            }
            else
            {
                // Sprites가 이전 Sprites와 같지않으면 Sprite를 변경
                if (listSprites[count] != listSprites[count - 1])
                {
                    animSprites.SetBool("Change", true);
                    yield return new WaitForSeconds(0.1f);
                    rendererSprite.GetComponent<SpriteRenderer>().sprite = listSprites[count];
                    animSprites.SetBool("Change", false);
                }
                // 이전과 모두 같으면 잠시 대기
                else
                {
                    yield return new WaitForSeconds(0.05f);
                }
            }
        }
        else
        {
            rendererDialogueWindow.GetComponent<SpriteRenderer>().sprite = listDialogueWindows[count];
            rendererSprite.GetComponent<SpriteRenderer>().sprite = listSprites[count];
        }

        // z키 연타 시 생기는 오류 방지
        keyActivated = true;

        // 대사 텍스트 출력
        for (int i = 0; i < listSentences[count].Length; i++)
        {
            text.text += listSentences[count][i]; // 1글자씩 출력

            // 사운드 출력
            if (i % 7 == 1)
            {
                theAudio.Play(typeSound);
            }

            yield return new WaitForSeconds(0.01f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isTalking && keyActivated)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                keyActivated = false;
                count++;
                text.text = "";
                theAudio.Play(enterSound);

                if (count == listSentences.Count)
                {
                    StopAllCoroutines();
                    ExitDialogue();
                }
                else
                {
                    StopAllCoroutines();
                    StartCoroutine(StartDialogueCoroutine());
                }
            }
        }
    }
}
