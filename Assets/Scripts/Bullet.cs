using UnityEngine;

public class Bullet : MonoBehaviour
{
    public const string TARGET_TAG = "Target";
    public const string WALL_TAG = "Wall";
    private void OnCollisionEnter(Collision objectWeHit)
    {
        if (objectWeHit.gameObject.CompareTag(TARGET_TAG))
        {
            print("hit " + objectWeHit.gameObject.name + "!");

            CreateBulletImpactEffect(objectWeHit);

            Destroy(gameObject);
        }

        if (objectWeHit.gameObject.CompareTag(WALL_TAG))
        {
            print("hit " + objectWeHit.gameObject.name + "!");

            CreateBulletImpactEffect(objectWeHit);

            Destroy(gameObject);
        }
    }

    void CreateBulletImpactEffect(Collision objectWeHit)
    {
        ContactPoint contactPoint = objectWeHit.contacts[0];

        GameObject hole = Instantiate(
            GlobalReferences.Instance.bulletImpactEffectPrefab,
            contactPoint.point,
            Quaternion.LookRotation(contactPoint.normal)
        );

        hole.transform.SetParent(objectWeHit.gameObject.transform);
    }
}
