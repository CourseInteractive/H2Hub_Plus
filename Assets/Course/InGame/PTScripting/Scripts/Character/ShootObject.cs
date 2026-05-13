
using UnityEngine;

namespace Course.PrototypeScripting
{
    public class ShootObject : MonoBehaviour
    {
        public GameObject bullet;
        public float shootForce;
        public bool limitedAmmunition;
        public string variableName;


        bool AmmunitionLeft()
        {
            return VariableManager.Instance.GetVariable(variableName) > 0;
        }

        void ReduceAmmunition()
        {
            VariableManager.Instance.SetVariable(variableName, VariableManager.Instance.GetVariable(variableName) - 1);
        }

        public void CreateAndShootBullet()
        {
            if (limitedAmmunition && !AmmunitionLeft())
                return;
            GameObject newBullet = Instantiate(bullet);
            newBullet.transform.position = transform.position + transform.forward * 1f;
            newBullet.SetActive(true);
            newBullet.GetComponent<Rigidbody>().AddForce(transform.forward * shootForce);
            if (limitedAmmunition)
                ReduceAmmunition();
        }
    }

}
