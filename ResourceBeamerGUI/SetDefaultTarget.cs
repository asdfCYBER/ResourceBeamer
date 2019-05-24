using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ResourceBeamer.GUI
{
    public class SetDefaultTarget : MonoBehaviour
    {
        //necessary, should be assigned in the inspector
        public GameObject DefaultVesselToggleGO;
        public GameObject SelectionGO;

        //to be accessed by other non-gui code
        public static bool SelectedVesselIsDefault;

        void Start()
        {
            DefaultVesselToggleGO.GetComponent<Toggle>().onValueChanged.AddListener(SetDefault);
        }

        void SetDefault(bool state)
        {
            SelectedVesselIsDefault = state;
            Debug.Log(SelectedVesselIsDefault);
        }
    }
}