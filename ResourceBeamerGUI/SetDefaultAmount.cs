using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ResourceBeamer.GUI
{
    public class SetDefaultAmount : MonoBehaviour
    {
        //necessary, should be assigned in the inspector
        public GameObject DefaultAmountToggleGO;
        public InputField AmountInputGO;

        //to be accessed by other non-gui code
        public static bool SelectedAmountIsDefault;

        void Start()
        {
            DefaultAmountToggleGO.GetComponent<Toggle>().onValueChanged.AddListener(SetDefault);
        }

        void SetDefault(bool state)
        {
            SelectedAmountIsDefault = state;
        }
    }
}