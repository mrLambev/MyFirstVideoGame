using UnityEngine;
using System.Collections.Generic;

public class BeerBottle : MonoBehaviour
{
    public List<Rigidbody> allParts = new List<Rigidbody>();

    public void Shatter()
    {
        // ����������� ����:
        SoundManager.Instance.BeerBottle_Break.Play();

        // ������ ��� �������� ������� ������������:
        foreach (Rigidbody part in allParts)
        {
            part.isKinematic = false;
        }

        // �������� ��������� �������:
        GetComponent<BoxCollider>().enabled = false;
    }
}
