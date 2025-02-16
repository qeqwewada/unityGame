using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthPos : MonoBehaviour
{

    public Image Arrow;
    public Transform target;
    /*public Transform Arrow3d;*/
    private Transform _lastTarget;
    Camera mainCamra => Camera.main;
    RectTransform indicator => Arrow.rectTransform;
    static Rect rect = new Rect(0, 0, 1, 1);
    private void Update()
    {
        if (target == null || mainCamra == null) return;
        Vector3 targetViewportPos = mainCamra.WorldToViewportPoint(target.position);

        if (targetViewportPos.z > 0 && rect.Contains(targetViewportPos))
        {
            /*     Debug.Log("!");
                 if (_lastTarget != target)
                 {
                     Arrow3d.parent = target;
                     Arrow3d.localPosition = Vector3.zero;
                     _lastTarget = target;
                 }
                 var v = target.position - mainCamra.transform.position;
                 Arrow3d.forward = v;
                 Arrow3d.localScale = Vector3.one * Mathf.Clamp(v.magnitude / 10, 0.1f, 1f);
                 Arrow.gameObject.SetActive(false);*/
            indicator.anchoredPosition = new Vector2((targetViewportPos.x - 0.5f) * Screen.width, (targetViewportPos.y - 0.5f) * Screen.height);
            indicator.rotation = Quaternion.identity;
        }
    }
}
