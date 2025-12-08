using UnityEngine;
using UnityEngine.SceneManagement;

public class playMain : MonoBehaviour
{
    public void SceneSwitch()
    {
        // Make sure this string matches your scene name EXACTLY (Case sensitive!)
        SceneManager.LoadScene(1);
    }
}