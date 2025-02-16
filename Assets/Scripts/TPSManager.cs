using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TPSManager : MonoBehaviour
{
    public string characterName;
    private FightPanel fightPanel;

    private void Awake()
    {
        if(Game.Instance!=null)
        characterName = Game.Instance.characterName;
        Vector3 randomPos = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
        PhotonNetwork.Instantiate(characterName, randomPos, Quaternion.identity);
    }
    void Start()
    {
        
        Texture2D t;
        t = (Texture2D)Resources.Load("Image/"+characterName);
        if (t != null) // �����Դ�Ƿ���سɹ�
        {
            Sprite temp = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0.5f, 0.5f)); // ʹ�� Vector2(0.5f, 0.5f) ��Ϊ���ĵ�
            GameObject.Find("FightUI").GetComponent<FightPanel>().character.sprite = temp;
        }
        else
        {
            Debug.LogError("Failed to load texture: Image/"+characterName);
        }
        Texture2D weapon;
        t = (Texture2D)Resources.Load("Image/" + characterName+"Wp");
        if (t != null) // �����Դ�Ƿ���سɹ�
        {
            Sprite temp = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0.5f, 0.5f)); // ʹ�� Vector2(0.5f, 0.5f) ��Ϊ���ĵ�
            GameObject.Find("FightUI").GetComponent<FightPanel>().Weapon.sprite = temp;
        }
        else
        {
            Debug.LogError("Failed to load texture: Image/" + characterName+"Wp");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.Instance.OpenPanel(UIConst.StopPanel);
        }
    }
}
