using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void SceneChange()
    {
        // GameManager �ν��Ͻ��� �ִ��� Ȯ���ϰ� ���ο� ���� �ʱ�ȭ �Լ��� ȣ���մϴ�.
        if (GameManager.Instance != null)
        {
            GameManager.Instance.InitializeNewGame(); // ���ο� ���� ���� �ʱ�ȭ (��� 1000, ü�� �ִ�ġ)
        }
        else
        {
            Debug.LogError("SceneChanger: GameManager �ν��Ͻ��� ã�� �� �����ϴ�! ���ο� ���� �ʱ�ȭ ����.");
        }

        // MainScene �ε�
        SceneManager.LoadScene("MainScene");
    }
}
