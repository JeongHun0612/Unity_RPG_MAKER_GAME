using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event1 : MonoBehaviour
{
    public Dialogue dialogue_1;
    public Dialogue dialogue_2;

    private DialogueManager theDM;
    private OrderManager theOrder;
    private PlayerManager thePlayer;

    private bool flag;


    // Start is called before the first frame update
    void Start()
    {
        theDM = FindObjectOfType<DialogueManager>();
        theOrder = FindObjectOfType<OrderManager>();
        thePlayer = FindObjectOfType<PlayerManager>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // 플레이어가 위(DirY == 1)를 바라보고 Z키를 누를때
        if (!flag && Input.GetKey(KeyCode.Z) && thePlayer.animator.GetFloat("DirY") == 1)
        {
            flag = true;
            StartCoroutine(EventCoroutine());
        }
    }

    IEnumerator EventCoroutine()
    {
        theOrder.PreLoadCharacter(); // 캐릭터 리스트 초기화
        theOrder.NotMove(); // 움직임 제어

        theDM.ShowDialogue(dialogue_1); // 이벤트(dialogue_1) 시작
        yield return new WaitUntil(() => !theDM.isTalking); // isTalking이 false가 될때까지 대기

        Debug.Log("dialogue_1 end");

        // 움직임 명령
        theOrder.Move("Player", "RIGHT");
        theOrder.Move("Player", "RIGHT");
        theOrder.Move("Player", "UP");

        yield return new WaitUntil(() => thePlayer.queue.Count == 0); // 움직임 명령 queue가 0이 될때까지

        theDM.ShowDialogue(dialogue_2); // 이벤트(dialogue_2) 시작
        yield return new WaitUntil(() => !theDM.isTalking); // isTalking이 false가 될때까지 대기

        theOrder.Move();
    }
}
