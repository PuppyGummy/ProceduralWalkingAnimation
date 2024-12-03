using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKFootManager : MonoBehaviour
{
    [SerializeField] private List<IKFootSolver> footSolvers = new List<IKFootSolver>();
    [SerializeField] LayerMask terrainLayer = default;
    [SerializeField] Transform body = default;
    [SerializeField] float speed = 1;
    [SerializeField] float stepDistance = 4;
    [SerializeField] float stepLength = 4;
    [SerializeField] float stepHeight = 1;
    [SerializeField] float stepDistanceTolarance = 0.01f;

    void Awake()
    {
        foreach (Transform child in transform)
        {
            IKFootSolver ikFootSolver = child.GetComponent<IKFootSolver>();
            ikFootSolver.terrainLayer = terrainLayer;
            ikFootSolver.body = body;
            ikFootSolver.speed = speed;
            ikFootSolver.stepDistance = stepDistance;
            ikFootSolver.stepLength = stepLength;
            ikFootSolver.stepHeight = stepHeight;
            ikFootSolver.stepDistanceTolarance = stepDistanceTolarance;
            footSolvers.Add(ikFootSolver);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
