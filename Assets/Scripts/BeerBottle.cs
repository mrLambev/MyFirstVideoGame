using UnityEngine;
using System.Collections.Generic;

public class BeerBottle : MonoBehaviour
{
    public List<Rigidbody> allParts = new List<Rigidbody>();

    public void Shatter()
    {
        // Проигрываем звук:
        SoundManager.Instance.BeerBottle_Break.Play();

        // Делаем все частички бутылки двигающимися:
        foreach (Rigidbody part in allParts)
        {
            part.isKinematic = false;
        }

        // Убирваем коллайдер объекта:
        GetComponent<BoxCollider>().enabled = false;
    }
}
