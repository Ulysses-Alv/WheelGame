using UnityEngine;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    [SerializeField] Slider music, soundfx;
    [SerializeField] Button closeOptions;
    void Start()
    {
        closeOptions.onClick.AddListener(() => gameObject.SetActive(false));
    }

    void Update()
    {

    }
}
