using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessManager : SingletonMono<PostProcessManager>
{
    public PostProcessVolume volume;
    private ChromaticAberration chromaticAberration;
    [SerializeField]private float speed;
    void Start()
    {
        chromaticAberration = volume.profile.GetSetting<ChromaticAberration>();
    }

    /// <summary>
    /// ɫ��Ч��
    /// </summary>
    public void ChromaticAberrationEF(float value)
    {
        StopAllCoroutines(); // ��ֹ��δ���
        StartCoroutine(StartChromaticAberrationEF(value));
    }

    IEnumerator StartChromaticAberrationEF(float value)
    {
        // ������value
        while (chromaticAberration.intensity<value)
        {
            yield return null;
            chromaticAberration.intensity.value += Time.deltaTime * speed;
        }
        // �ݼ���0
        while (chromaticAberration.intensity > 0)
        {
            yield return null;
            chromaticAberration.intensity.value -= Time.deltaTime * speed;
        }
    }



}
