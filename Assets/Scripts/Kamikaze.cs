using UnityEngine;

public class Kamikaze : MonoBehaviour
{
    [SerializeField] private Player player;
    private Perception perception;

    public Player SetPlayer { set { player = value; } }

    private bool isSeeingPlayer = false;

    private void Awake()
    {
        perception = GetComponent<Perception>();
    }

    private void Update()
    {
        isSeeingPlayer = perception.GetIsTargetVisible;

        if (isSeeingPlayer)
        {
            Debug.Log("Te veo");
        }
        else
        {
            Debug.Log("No te veo");
        }
    }
}
