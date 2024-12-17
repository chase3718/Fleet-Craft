using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SailingUIManager : MonoBehaviour
{
    Button backBtn;
    // Start is called before the first frame update
    void Awake()
    {
        UIDocument doc = GetComponent<UIDocument>();
        backBtn = doc.rootVisualElement.Q<Button>("BackBtn");
        backBtn.clicked += () => {
            SceneManager.LoadScene( "Dock" );
            };
        
        backBtn.style.width = backBtn.style.height;
    }
}
