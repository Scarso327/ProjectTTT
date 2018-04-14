using UnityEngine;

public class CheckSpawn : MonoBehaviour
{
    public bool checkSpawn
    {
        get
        {
            if (Physics.CheckSphere(this.gameObject.transform.position, 5.0F))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}