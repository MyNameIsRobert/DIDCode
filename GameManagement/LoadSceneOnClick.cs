using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class LoadSceneOnClick : MonoBehaviour {

	public GameObject loadingImage;

	public void LoadByIndex(int sceneIndex){
		SceneManager.LoadScene (sceneIndex);
	}
}
