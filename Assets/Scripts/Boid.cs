using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public float zoneRepulsion = 5;
    public float zoneAlignement = 9;
    public float zoneAttraction = 50;
    public float hauteurSol = 0;

    public float forceRepulsion = 15;
    public float forceAlignement = 3;
    public float forceAttraction = 20;
    public float forceRejetSol = 100;

    public Vector3 target = new Vector3();
    public float forceTarget = 15;
    public bool goToTarget = false;
    public bool stopToTarget = false;
    private bool atTarget = false;

    public Vector3 velocity = new Vector3();
    public float maxSpeed = 20;
    public float minSpeed = 10;

    public bool drawGizmos = true;
    public bool drawLines = true;

    // Update is called once per frame
    void Update()
    {
        Vector3 sumForces = new Vector3();
        Color colorDebugForce = Color.black;
        float nbForcesApplied = 0;

        foreach (Boid otherBoid in BoidManager.sharedInstance.roBoids)
        {
            Vector3 vecToOtherBoid = otherBoid.transform.position - transform.position;

            Vector3 forceToApply = new Vector3();

            //Si on doit prendre en compte cet autre boid (plus grande zone de perception)
            if (vecToOtherBoid.sqrMagnitude < zoneAttraction * zoneAttraction)
            {
                //Si on est entre attraction et alignement
                if (vecToOtherBoid.sqrMagnitude > zoneAlignement * zoneAlignement)
                {
                    //On est dans la zone d'attraction uniquement
                    forceToApply = vecToOtherBoid.normalized * forceAttraction;
                    float distToOtherBoid = vecToOtherBoid.magnitude;
                    float normalizedDistanceToNextZone = ((distToOtherBoid - zoneAlignement) / (zoneAttraction - zoneAlignement));
                    float boostForce = (4 * normalizedDistanceToNextZone);
                    if (!goToTarget) //Encore plus de cohésion si pas de target
                        boostForce *= boostForce;
                    forceToApply = vecToOtherBoid.normalized * forceAttraction * boostForce;
                    colorDebugForce += Color.green;
                }
                else
                {
                    //On est dans alignement, mais est on hors de répulsion ?
                    if (vecToOtherBoid.sqrMagnitude > zoneRepulsion * zoneRepulsion)
                    {
                        //On est dans la zone d'alignement uniquement
                        forceToApply = otherBoid.velocity.normalized * forceAlignement;
                        colorDebugForce += Color.blue;
                    }
                    else
                    {
                        //On est dans la zone de repulsion
                        float distToOtherBoid = vecToOtherBoid.magnitude;
                        float normalizedDistanceToPreviousZone = 1 - (distToOtherBoid / zoneRepulsion);
                        float boostForce = (4 * normalizedDistanceToPreviousZone);
                        forceToApply = vecToOtherBoid.normalized * -1 * (forceRepulsion * boostForce);
                        colorDebugForce += Color.red;

                    }
                }

                sumForces += forceToApply;
                nbForcesApplied++;
            }
        }

        //On fait la moyenne des forces, ce qui nous rend indépendant du nombre de boids
        sumForces /= nbForcesApplied;

        //On ajoute le rejet du sol
        float distSol = Mathf.Max(0,transform.position.y - hauteurSol);
        if (distSol < 5)
        {
            float forceRejet = Mathf.Pow(1.0f - (distSol / 5), 2) * forceRejetSol;
            sumForces += new Vector3(0, forceRejet, 0);
        }

        //Si on a une target, on l'ajoute
        float distToTarget = 0;
        if (goToTarget)
        {
            Vector3 vecToTarget = target - transform.position;
            distToTarget = vecToTarget.magnitude;
            if (distToTarget < 0.5f)
            {
                goToTarget = false;
                atTarget = true;
            }
            else
            {
                atTarget = false;
                Vector3 forceToTarget = vecToTarget.normalized * forceTarget;
                if (distToTarget < 5 && stopToTarget)
                    forceToTarget *= 10;
                sumForces += forceToTarget;
                colorDebugForce += Color.magenta;
                nbForcesApplied++;
                if (drawLines)
                    Debug.DrawLine(transform.position, target, Color.magenta);
            }
        }

        //Debug
        if (drawLines)
            Debug.DrawLine(transform.position, transform.position + sumForces, colorDebugForce / nbForcesApplied);

        //On freine
        velocity += -velocity * 10 * Vector3.Angle(sumForces, velocity) / 180.0f * Time.deltaTime;

        //on applique les forces
        velocity += sumForces * Time.deltaTime;

        //On limite la vitesse
        if (velocity.sqrMagnitude > maxSpeed * maxSpeed)
            velocity = velocity.normalized * maxSpeed;
        if (velocity.sqrMagnitude < minSpeed * minSpeed)
            velocity = velocity.normalized * minSpeed;

        //Si on doit freiner sur la cible, on limite la vitesse
        if ((goToTarget || atTarget) && stopToTarget)
            if (distToTarget < 3)
                velocity = velocity.normalized * distToTarget/10.0f;


        //On regarde dans la bonne direction        
        if (velocity.sqrMagnitude > 0)
            transform.LookAt(transform.position + velocity);

        //Debug
        if (drawLines)
            Debug.DrawLine(transform.position, transform.position + velocity, Color.blue);

        //Deplacement du boid
        transform.position += velocity * Time.deltaTime;
    }

    void OnDrawGizmosSelected()
    {
        if (drawGizmos)
        {
            // Répulsion
            Gizmos.color = new Color(1, 0, 0, 1.0f);
            Gizmos.DrawWireSphere(transform.position, zoneRepulsion);
            // Alignement
            Gizmos.color = new Color(0, 1, 0, 1.0f);
            Gizmos.DrawWireSphere(transform.position, zoneAlignement);
            // Attraction
            Gizmos.color = new Color(0, 0, 1, 1.0f);
            Gizmos.DrawWireSphere(transform.position, zoneAttraction);
        }
    }
}
