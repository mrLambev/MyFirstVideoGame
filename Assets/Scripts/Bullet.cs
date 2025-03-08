using UnityEngine;

public class Bullet : MonoBehaviour
{
    public const string TARGET_TAG = "Target";
    public const string WALL_TAG = "Wall";
    public const string BEER_BOTTLE_TAG = "Beer Bottle";
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

        if (objectWeHit.gameObject.CompareTag(BEER_BOTTLE_TAG))
        {
            print("hit a beer bottle part named " + objectWeHit.gameObject.name + "!");

            objectWeHit.gameObject.GetComponent<BeerBottle>().Shatter();

            // ѕосле попадани€ по бутылке пул€ не будет уничтожена
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
