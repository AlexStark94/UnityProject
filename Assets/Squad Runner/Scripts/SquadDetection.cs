using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JetSystems;

public class SquadDetection : MonoBehaviour
{
    [Header(" Managers ")]
    [SerializeField] private SquadFormation squadFormation;
    [SerializeField] private Runner runner;

    [Header(" Settings ")]
    [SerializeField] private LayerMask doorLayer;
    [SerializeField] private LayerMask finishLayer;
    [SerializeField] private LayerMask obstacleLayer;

   

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (UIManager.IsGame())
        {
            DetectDoors();
            DetectFinishLine();
            DetectionObstacles();

        }
    }

    

    private void DetectDoors()
    {
        Collider[] detectedDoors = Physics.OverlapSphere(transform.position, squadFormation.GetSquadRadius(), doorLayer);

        if (detectedDoors.Length <= 0) return;

        Collider collidedDoorCollider = detectedDoors[0];
        Door collidedDoor = collidedDoorCollider.GetComponentInParent<Door>();

        int runnersAmountToAdd = collidedDoor.GetRunnersAmountToAdd(collidedDoorCollider, transform.childCount);
        squadFormation.AddRunners(runnersAmountToAdd);
        
    }

    private void DetectFinishLine()
    {
        if (Physics.OverlapSphere(transform.position, 1, finishLayer).Length > 0)
        {
            FindObjectOfType<FinishLine>().PlayConfettiParticles();
            SetLevelComplete();

        }

    }

    private void DetectionObstacles()
    {
        if (Physics.OverlapSphere(transform.position, 0.5f, obstacleLayer).Length > 0)
        {
            runner.Explode();
            
        }
            
        
        
    } 
     
    private void SetLevelComplete()
    {
        UIManager.setLevelCompleteDelegate?.Invoke(3);
        
    }


    


}
