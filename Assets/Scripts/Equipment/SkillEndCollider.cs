using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEndCollider : MonoBehaviour
{
    public List<GameObject> slashes = new List<GameObject>();

    [SerializeField] private Collider2D col;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnSkillEffects());
    }

    private void Update()
    {
        //Make the slashes slowly shrink
        if (slashes[0].activeSelf)
        {
            foreach (GameObject slash in slashes)
            {
                if (slash.transform.localScale.y <= 0)
                {
                    continue;
                }

                slash.transform.localScale = new Vector3(slash.transform.localScale.x, slash.transform.localScale.y - 0.1f * Time.deltaTime * 10, slash.transform.localScale.z);
            }
        }
    }

    public IEnumerator SpawnSkillEffects()
    {
        foreach (GameObject slash in slashes)
        {
            yield return new WaitForSeconds(0.05f);
            col.enabled = false;
            col.enabled = true;
            slash.transform.SetPositionAndRotation(
            new Vector3(Random.Range(col.bounds.min.x + 1, col.bounds.max.x - 1), Random.Range(col.bounds.min.y + 1, col.bounds.max.y - 1), 0), Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 179))));
            slash.SetActive(true);
        }
        
        Destroy(gameObject, 2.5f);
    }
}
