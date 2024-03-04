using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private GameObject NPCUI1;
    [SerializeField] private GameObject NPCUI2;
    [SerializeField] private GameObject NPCUI3;
    [SerializeField] private GameObject Store;
    //public GameObject Text1;
    //public GameObject Text2;

    private void OnMouseDown()
    {
        NPCUI1.SetActive(true);
    }

    public void ExitBtn()
    {
        NPCUI2.SetActive(false);
    }
    public void StoreBtn()
    {
        NPCUI1.SetActive(false);
        Store.SetActive(true);
    }
    public void NPCUI2Btn()
    {
        NPCUI1.SetActive(false);
        NPCUI2.SetActive(true);
    }

    public void StroeExitBtn()
    {
        Store.SetActive(false);
    }

    public void FullHealthBtn()
    {
        NPCUI3.SetActive(false);
    }
}
