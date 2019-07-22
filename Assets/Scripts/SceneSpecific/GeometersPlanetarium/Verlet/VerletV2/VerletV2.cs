﻿public class VerletV2 : UnityEngine.MonoBehaviour
{
    public float bodyScale = 1; //Multipliciative scale of the size of the bodies in the system
    public string CenterBody = "Sol";
    public UnityEngine.Vector3d CenterBodyOffset = new UnityEngine.Vector3d(0, 0, 0);

    //Below are DEBUG variables
    private int counter; //Counter of the number of thread loops

    public UnityEngine.GameObject[]
        massObject; //A list of the rest of the bodies (Note to self: Bring down to nLog(n) using control script)

    public double masterDaysCounter;
    public double masterTimeCounter;
    private int minicounter; //Counter of the number of update calls
    private VerletV2Thread[] multithreadedJobs = new VerletV2Thread[0]; //List of all of the active threads
    private int multiThreadFlag; //This flag indicates how many threads have been completed
    private float previousTimeStep; //The previous timestep for time corrected verlet
    public float scale = 1; //Numerical scale of the distance between objects
    private System.Collections.Generic.List<string> threadIdent = new System.Collections.Generic.List<string>();
    public float timeStep = 1; //The length of the timestep in seconds

    // Use this for initialization
    private void Start()
    {
        previousTimeStep = timeStep; //Initial condition
        runThreads();
    }

    // Update is called once per frame
    private void Update()
    {
        VerletObjectV2 center = (VerletObjectV2) UnityEngine.GameObject.Find(CenterBody).GetComponent("VerletObjectV2");
        CenterBodyOffset = center.position;
        minicounter++;
        masterDaysCounter = masterTimeCounter / 86400;
        //Debug.Log("On frame : "+minicounter+", "+counter+" loops have completed.");
        runThreads();
    }

    private void runThreads()
    {
        float tmpTs = timeStep; //Temperory timestep to prevent input mess
        if (multiThreadFlag == multithreadedJobs.Length && timeStep != 0)
        {
            //If the threads have completed
            float tmppTs = previousTimeStep;
            masterTimeCounter += previousTimeStep;
            counter += 1; //Increase the counter of how many loops have gone through
            multiThreadFlag = 0; //Flag that there are threads running
            massObject =
                UnityEngine.GameObject
                    .FindGameObjectsWithTag("massObject"); //Gathers all of the massObjects for calculation
            float[] masses = new float[massObject.Length]; //Gathers all of the masses in order
            UnityEngine.Vector3d[]
                positions = new UnityEngine.Vector3d[massObject
                    .Length]; //Gathers all of the vectors of the objects in order
            for (int i = 0; i < massObject.Length; i++)
            {
                //For every object
                VerletObjectV2 obj = (VerletObjectV2) massObject[i].GetComponent("VerletObjectV2");
                masses[i] = obj.mass;
                positions[i] = obj.position;
            }

            multithreadedJobs = new VerletV2Thread[massObject.Length]; //Create thread objects
            for (int i = 0; i < massObject.Length; i++)
            {
                //For each thread object
                multithreadedJobs[i] = new VerletV2Thread();
                VerletV2Thread thisJob = multithreadedJobs[i];
                thisJob.ThreadName = massObject[i].name + i; //Assign name
                thisJob.thisIndex = i; //Assign index (Unused I think)
                thisJob.m = masses[i]; //Assign mass
                thisJob.p = positions[i]; //Assign position of object
                thisJob.pp =
                    ((VerletObjectV2) massObject[i].GetComponent("VerletObjectV2"))
                    .previousPosition; //Assign previous position
                thisJob.m2 = masses; //Assign mass of every object
                thisJob.p2 = positions; //Assign psoition of every object
                thisJob.ts = tmpTs; //Assign timestep for the simulation
                thisJob.pts = tmppTs; //Assign previous timestep
                multithreadedJobs[i].Start(); //Start thread
                StartCoroutine("finished", multithreadedJobs[i]); //Start end coroutine
            }

            ///for(int i = 0; i < multithreadedJobs.Length; i++) {
            //	multithreadedJobs[i].Start();
            //	StartCoroutine ("finished" ,multithreadedJobs[i]);
            //}
            previousTimeStep = tmpTs;
        }
    }

    private System.Collections.IEnumerator finished(VerletV2Thread myJob)
    {
        yield return StartCoroutine(myJob.WaitFor());
        //Debug.Log("finished "+myJob.ThreadName+"	it was: "+myJob.p);
        UnityEngine.Vector3d indestructable = new UnityEngine.Vector3d(myJob.p.x, myJob.p.y, myJob.p.z);
        ((VerletObjectV2) massObject[myJob.thisIndex].GetComponent("VerletObjectV2")).newPos(indestructable);
        multiThreadFlag += 1;
        runThreads();
    }
}