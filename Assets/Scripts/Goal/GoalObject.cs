using UnityEngine;
using UnityEngine.Events;

public class GoalObject : MonoBehaviour
{
    public static UnityEvent PlayerInGoal = new UnityEvent();

    private void OnTriggerEnter(Collider other)
    {
        print("Player reached Goal");
        if (other.GetComponent<CharacterController>())
        {
            PlayerInGoal?.Invoke();
        }
    }
}
