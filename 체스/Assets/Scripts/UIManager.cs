using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnClickSignupBtn()
    {
        SceneManager.LoadScene("SignupScene", LoadSceneMode.Single);
    }
    public void OnClickBackBtn()
    {
        SceneManager.LoadScene("LoginScene", LoadSceneMode.Single);
    }
    public void OnClickSignupCompleteBtn()
    {
        SceneManager.LoadScene("LoginScene", LoadSceneMode.Single);
    }
}
