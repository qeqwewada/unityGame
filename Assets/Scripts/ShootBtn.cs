using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using StarterAssets;

public class ShootBtn : MonoBehaviour,IPointerDownHandler,IPointerUpHandler
{
    public FightPanel fightPanel;

    public void OnPointerDown(PointerEventData eventData)
    {
        if(fightPanel.currentPlayer.TryGetComponent(out StarterAssetsInputs starterAssetsInputs))
        {
            starterAssetsInputs.shoot = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (fightPanel.currentPlayer.TryGetComponent(out StarterAssetsInputs starterAssetsInputs))
        {
            starterAssetsInputs.shoot = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
