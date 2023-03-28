using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVSystem : MonoBehaviour
{
    public float viewRadius;
    public float viewAngle;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public List<Transform> visibleTargets = new List<Transform>();

    
    // Start is called before the first frame update
    public virtual void Start()
    {
        StartCoroutine("FindTargetsWithDelay", 0.2f);
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    void FindVisibleTargets()
    {
        visibleTargetColor(Color.white);
        visibleTargets.Clear();

        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        for(int i =0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            if(Vector3.Angle(transform.forward , dirToTarget) <viewAngle/2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);

                if(!Physics.Raycast(transform.position,dirToTarget,dstToTarget,obstacleMask))
                {
                    visibleTargets.Add(target);
                }
            }
        }
        visibleTargetColor(Color.green);
    }


   public Vector3 DirFromAngle(float angleInDegress, bool anglesGlobal)
    {
        if(!anglesGlobal)
        {
            angleInDegress += transform.eulerAngles.y;
        }

        return new Vector3(Mathf.Sin(angleInDegress * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegress * Mathf.Deg2Rad));
    }

    void visibleTargetColor(Color color)
    {
        for(int i = 0; i < visibleTargets.Count; i++)
        {
            visibleTargets[i].GetComponent<Renderer>().material.SetColor("_Color", color);
        }
    }
}
