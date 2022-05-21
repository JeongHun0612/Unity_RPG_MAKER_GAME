using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    public string characterName;

    public float speed;
    public int walkCount;
    protected int currentWalkCount;

    private bool notCoroutine = false;
    protected Vector3 vector;

    public Queue<string> queue;

    public BoxCollider2D boxCollider;
    public LayerMask layerMask;
    public Animator animator;

    public void Move(string _direction, int _frequency = 5)
    {
        queue.Enqueue(_direction);
        if (!notCoroutine)
        {
            notCoroutine = true;
            StartCoroutine(MoveCoroutine(_direction, _frequency));
        }
    }

    IEnumerator MoveCoroutine(string _direction, int _frequency)
    {
        while (queue.Count != 0)
        {
            // 다음 이동 간 대기시간
            switch (_frequency)
            {
                case 1:
                    yield return new WaitForSeconds(4f);
                    break;
                case 2:
                    yield return new WaitForSeconds(3f);
                    break;
                case 3:
                    yield return new WaitForSeconds(2f);
                    break;
                case 4:
                    yield return new WaitForSeconds(1f);
                    break;
                case 5:
                    break;
            }

            // 입력받은 방향 저장 및 vector값 초기화
            string direction = queue.Dequeue();
            vector.Set(0, 0, vector.z);

            // 입력받은 방향으로 vector값 저장
            switch (direction)
            {
                case "UP":
                    vector.y = 1f;
                    break;
                case "DOWN":
                    vector.y = -1f;
                    break;
                case "RIGHT":
                    vector.x = 1f;
                    break;
                case "LEFT":
                    vector.x = -1f;
                    break;
            }

            // 이동한 방향으로 애니메이션 실행
            animator.SetFloat("DirX", vector.x);
            animator.SetFloat("DirY", vector.y);

            // 움직이는 방향에 충돌체가 있으면 정지
            while (true)
            {
                bool checkCollsionFlag = CheckCollsion();
                if (checkCollsionFlag)
                {
                    animator.SetBool("Walking", false);
                    yield return new WaitForSeconds(1f);
                }
                else
                {
                    break;
                }
            }

            // BoxCollider를 움직일 방향으로 미리 이동
            boxCollider.offset = new Vector2(vector.x * 0.7f * speed * walkCount, vector.y * 0.7f * speed * walkCount);

            animator.SetBool("Walking", true);

            // 실제 위치 값 변경 (이동)
            while (currentWalkCount < walkCount)
            {
                transform.Translate(vector.x * speed, vector.y * speed, 0);
                currentWalkCount++;

                // BoxCollider 위치 값 초기화
                if (currentWalkCount == 12)
                    boxCollider.offset = Vector2.zero;

                yield return new WaitForSeconds(0.01f);
            }
            currentWalkCount = 0;

            if (_frequency != 5)
                animator.SetBool("Walking", false);
        }
        animator.SetBool("Walking", false);
        notCoroutine = false;
    }

    // 이동할 위치에 충돌체가 있는 확인하는 함수
    protected bool CheckCollsion()
    {
        RaycastHit2D hit;

        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(vector.x * speed * walkCount, vector.y * speed * walkCount);

        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, layerMask);
        boxCollider.enabled = true;

        if (hit.transform != null)
            return true;

        return false;
    }
}
