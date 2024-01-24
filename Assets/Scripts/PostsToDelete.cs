using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; // Make sure to include this namespace


// The purpose of this script is to store the cloud anchor IDs that have been selected on the delete posts page.
public class PostsToDelete : MonoBehaviour
{
    public HashSet<string> postsToDelete = new HashSet<string>();
    public List<string> alist = new List<string>();

    void Update()
    {
        alist = postsToDelete.ToList();
    }
}
