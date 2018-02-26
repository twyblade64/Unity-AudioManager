using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour {
    // Put Example scenes for this to work.
	public void GoToScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }
}
