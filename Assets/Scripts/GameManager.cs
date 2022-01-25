using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] List<GameObject> chunks = new List<GameObject>();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            chunks[0].SetActive(true);
            chunks[1].SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            chunks[0].SetActive(false);
            chunks[1].SetActive(true);
        }
    }
}
