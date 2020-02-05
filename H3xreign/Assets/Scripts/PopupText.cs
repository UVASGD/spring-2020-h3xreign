using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopupText : MonoBehaviour
{
    public float impulse;
    [HideInInspector]
    public TextMeshPro text;
    Camera m_Camera;
    // Start is called before the first frame update
    void Awake()
    {
        m_Camera = Camera.main;
        text = GetComponent<TextMeshPro>();
        GetComponent<Rigidbody>().AddForce(Random.onUnitSphere + Vector3.up * impulse, ForceMode.Impulse);

        Destroy(gameObject, 2);
    }

    // Code from http://wiki.unity3d.com/index.php?title=CameraFacingBillboard&_ga=2.152504633.106683565.1579371007-436228330.1530937297
    void LateUpdate()
    {
        transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.forward,
            m_Camera.transform.rotation * Vector3.up);
    }

    public void Say(string popupText)
    {
        text.text = popupText;
    }
}
