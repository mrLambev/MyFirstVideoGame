using System;
using UnityEngine;

public class Throwable : MonoBehaviour
{
    [SerializeField] float delay = 3f;
    [SerializeField] float damageRadius = 50f;
    [SerializeField] float explosionForce = 4000f;

    float countdown;

    bool hasExploded = false;
    public bool hasBeenThrown = false;

    public enum ThrowableType 
    { 
        HighExplosiveGrenade
    }

    public ThrowableType throwableType;

    private void Start()
    {
        countdown = delay;
    }

    private void Update()
    {
        if (hasBeenThrown)
        {
            countdown -= Time.deltaTime;
            if (countdown <= 0f && hasExploded == false)
            {
                Explode();
            }
        }
    }

    // �����, ������������� ��� ������:
    private void Explode()
    {
        // ��������� ������:
        GetThrowableEffect();

        // ���������� ������ �� �����:
        Destroy(gameObject);

        hasExploded = true;
    }

    // ����� ��� ��������� ������� � ����������� �� ����:
    private void GetThrowableEffect()
    {
        switch (throwableType)
        {
            case ThrowableType.HighExplosiveGrenade:
                HighExplosiveEffect();
                break;
            default:
                print("UNKNOWN THROWABLE TYPE!");
                break;
        }
    }

    // ����� ��� ������� ������� �� ���������� �������:
    private void HighExplosiveEffect()
    {
        // ���������� ������:
        // �������� ���������� ������ ������ ���������� �������:
        GameObject explosionEffect = GlobalReferences.Instance.highExplosionGrenadeEffect;

        // ������� ������ ������ ���������� ������� � �����:
        Instantiate(explosionEffect, transform.position, transform.rotation);



        // ���������� ������:
        // �������� ����: Physics.OverlapSphere() ���������� ��� Collider, ������� ������ ������
        // �������� Collider ���� �������� � ������� ������:
        Collider[] collidersInRange = Physics.OverlapSphere(transform.position, damageRadius);

        // ���������� �� ���� Collider, ������� ��������� ��������:
        foreach (Collider colliderInRange in collidersInRange)
        {
            // �������� �������� Rigidbody � �������, �������� ����������� ������ Collider:
            Rigidbody rb = colliderInRange.GetComponent<Rigidbody>();

            // ���� ������� �������� Rigidbody, �� ��������� � ������� �������� ����:
            if(rb != null)
            {
                // �������� ����: ����� AddExplosionForce() ������� Rigidbody ��������� � ���� ���� ������
                rb.AddExplosionForce(explosionForce, transform.position, damageRadius);
            }
        }
    }
}
