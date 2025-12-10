using System.Collections;
using UnityEngine;

public class SetActiveButtonScript : MonoBehaviour
{
    public GameObject targetObject;

    public void ToogleActiveAfterDelay(float delay)
    {
        StartCoroutine(ToogleActiveCoroutine(delay));
    }

    private IEnumerator ToogleActiveCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        targetObject.SetActive(!targetObject.activeSelf);
        gameObject.SetActive(!gameObject.activeSelf);

    }

}
