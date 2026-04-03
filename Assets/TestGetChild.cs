using System.Collections;
using UnityEngine;

public class TestGetChild : MonoBehaviour
{
    private TestPrent test;

    [SerializeField] private GameObject childRotating;
    [SerializeField] private GameObject parentScaling;

    private bool pauseAnimation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        pauseAnimation = true;
        StartCoroutine(RotationAnim());
        StartCoroutine(ScalingAnim());
        StartCoroutine(ScalingConterAnim());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator RotationAnim()
    {
        while (pauseAnimation)
        {
            float rotationTarget = 0f;
            float start = 0f;
            float end = 360f;
            float time = 0f;
            while (rotationTarget != 360f)
            {
                time += 0.05f;
                rotationTarget = Mathf.Lerp(start, end, time);
                childRotating.transform.eulerAngles = new Vector3(0, 0, rotationTarget);
                yield return null;
            }
        }
    }

    IEnumerator ScalingAnim()
    {
        float start = 2f;
        float time = 0f;
        float end = 3f;
        while (pauseAnimation)
        {
            float ScaleTarget = 1f;

            time = 0;
            while (ScaleTarget != 3)
            {
                time += 0.01f;
                ScaleTarget = Mathf.Lerp(start, end, time);
                parentScaling.transform.localScale = new Vector3(1, ScaleTarget, 1);
                yield return null;
            }

            time = 0;
            while (ScaleTarget != 2)
            {
                time += 0.01f;
                ScaleTarget = Mathf.Lerp(end, start, time);
                parentScaling.transform.localScale = new Vector3(1, ScaleTarget, 1);
                yield return null;
            }
        }
    }

    IEnumerator ScalingConterAnim()
    {
        float start = 2f;
        float time = 0f;
        float end = 1f;
        while (pauseAnimation)
        {
            float ScaleTarget = 1f;

            time = 0;
            while (ScaleTarget != 1)
            {
                time += 0.01f;
                ScaleTarget = Mathf.Lerp(start, end, time);
                childRotating.transform.localScale = new Vector3(1, ScaleTarget, 1);
                yield return null;
            }

            time = 0;
            while (ScaleTarget != 2)
            {
                time += 0.01f;
                ScaleTarget = Mathf.Lerp(end, start, time);
                childRotating.transform.localScale = new Vector3(1, ScaleTarget, 1);
                yield return null;
            }
        }
    }
}
