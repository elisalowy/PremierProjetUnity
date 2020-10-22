using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    private static BoidManager instance = null;
    public static BoidManager sharedInstance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<BoidManager>();
            }
            return instance;
        }
    }

    public Boid prefabBoid;
    public float nbBoids = 100;
    public float startSpeed = 1;
    public float startSpread = 10;

    public float maxDistBoids = 30;
    public float hauteurSol = 0;

    public float periodRetargetBoids = 6;
    public float periodNoTargetBoids = 3;
    public float periodLandedBoids = 6;
    private float timerRetargetBoids = 0;
    private enum StateBoids
    {
        RANDOM_FLIGHT,
        HAS_TARGET,
        LANDING
    }
    private StateBoids nextStateBoids = StateBoids.RANDOM_FLIGHT;

    public bool setBirdView = true;

    private List<Boid> boids = new List<Boid>();
    public ReadOnlyCollection<Boid> roBoids
    {
        get { return new ReadOnlyCollection<Boid>(boids); }
    }

    void Start()
    {
        for (int i = 0; i < nbBoids; i++)
        {
            Boid b = GameObject.Instantiate<Boid>(prefabBoid);
            Vector3 positionBoid = Random.insideUnitSphere * startSpread;
            positionBoid.y = Mathf.Abs(positionBoid.y); //Ne pas créer des oiseaux sous 0, on imagine que ce sera le sol.
            b.transform.position = positionBoid;
            b.velocity = (positionBoid - transform.position).normalized * startSpeed;
            b.transform.parent = this.transform;
            b.maxSpeed *= Random.Range(0.95f, 1.05f);
            boids.Add(b);
        }
    }

    void Update()
    {
        
        if (setBirdView)
        {
            
            Camera.main.transform.parent = boids[0].transform;
            Camera.main.transform.rotation = boids[0].transform.rotation;
            Camera.main.transform.localPosition = new Vector3();
            boids[0].GetComponentInChildren<Renderer>().enabled = false;
        }
        else
        {
            Camera.main.transform.parent = null;
            boids[0].GetComponentInChildren<Renderer>().enabled = true;
        }           

        timerRetargetBoids -= Time.deltaTime;
        if (timerRetargetBoids <= 0)
        {
            Vector3 target = Random.insideUnitSphere * maxDistBoids;
            target.y = Mathf.Max(Mathf.Abs(target.y), 10);

            switch (nextStateBoids)
            {
                case StateBoids.HAS_TARGET:
                    timerRetargetBoids = periodRetargetBoids;
                    break;
                case StateBoids.RANDOM_FLIGHT:
                    timerRetargetBoids = periodNoTargetBoids;
                    break;
                case StateBoids.LANDING:
                    timerRetargetBoids = periodLandedBoids;
                    break;
            }
            
            foreach (Boid b in boids)
            {
                b.hauteurSol = hauteurSol;

                switch (nextStateBoids)
                {
                    case StateBoids.HAS_TARGET:
                        b.goToTarget = false;
                        b.stopToTarget = false;
                        if (Random.Range(0.0f, 1.0f) < 0.3f)
                        {
                            b.target = target;
                            b.goToTarget = true;
                        }
                        break;
                    case StateBoids.RANDOM_FLIGHT:
                        b.goToTarget = false;
                        b.stopToTarget = false;
                        break;

                    case StateBoids.LANDING:
                        b.target = b.transform.position;
                        b.target.y = hauteurSol;
                        b.stopToTarget = true;
                        b.goToTarget = true;
                        break;
                }
            }

            switch (nextStateBoids)
            {
                case StateBoids.HAS_TARGET:
                    nextStateBoids = StateBoids.RANDOM_FLIGHT;
                    break;
                case StateBoids.RANDOM_FLIGHT:
                    nextStateBoids = StateBoids.LANDING;
                    break;
                case StateBoids.LANDING:
                    nextStateBoids = StateBoids.HAS_TARGET;
                    break;
            }
        }
    }
}
