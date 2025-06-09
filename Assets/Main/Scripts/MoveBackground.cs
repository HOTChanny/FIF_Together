using UnityEngine;

public class LoopingBackground : MonoBehaviour
{
    public float speed = 2f;  // 배경이 왼쪽으로 움직일 속도
    private float backgroundWidth;  // 스프라이트 가로 길이

    void Start()
    {
        // 현재 배경 이미지의 가로 길이를 구함
        backgroundWidth = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        // 왼쪽으로 이동 (오른쪽 시작 → 왼쪽 이동)
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        // 배경이 왼쪽으로 완전히 벗어나면
        if (transform.position.x <= -backgroundWidth)
        {
            // 배경을 오른쪽으로 재배치 (2개 붙어있다고 가정)
            float offset = backgroundWidth * 2f;
            transform.position = new Vector3(transform.position.x + offset, transform.position.y, transform.position.z);
        }
    }
}
