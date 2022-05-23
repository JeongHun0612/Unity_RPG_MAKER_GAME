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
        // �÷��̾ ��(DirY == 1)�� �ٶ󺸰� ZŰ�� ������
        if (!flag && Input.GetKey(KeyCode.Z) && thePlayer.animator.GetFloat("DirY") == 1)
        {
            flag = true;
            StartCoroutine(EventCoroutine());
        }
    }

    IEnumerator EventCoroutine()
    {
        theOrder.PreLoadCharacter(); // ĳ���� ����Ʈ �ʱ�ȭ
        theOrder.NotMove(); // ������ ����

        theDM.ShowDialogue(dialogue_1); // �̺�Ʈ(dialogue_1) ����
        yield return new WaitUntil(() => !theDM.isTalking); // isTalking�� false�� �ɶ����� ���

        Debug.Log("dialogue_1 end");

        // ������ ���
        theOrder.Move("Player", "RIGHT");
        theOrder.Move("Player", "RIGHT");
        theOrder.Move("Player", "UP");

        yield return new WaitUntil(() => thePlayer.queue.Count == 0); // ������ ��� queue�� 0�� �ɶ�����

        theDM.ShowDialogue(dialogue_2); // �̺�Ʈ(dialogue_2) ����
        yield return new WaitUntil(() => !theDM.isTalking); // isTalking�� false�� �ɶ����� ���

        theOrder.Move();
    }
}
