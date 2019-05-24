using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ResourceBeamer.GUI
{
    public class SetTarget : MonoBehaviour
    {
        //necessary, should be assigned in the inspector
        public GameObject ScrollViewButtonGO;
        public GameObject SelectionGO;

        //to be accessed by other non-gui code
        public static string SelectedVessel;
        public static string SelectedVesselName;
        public static string TextContent;

        void Start()
        {
            ScrollViewButtonGO.GetComponent<Button>().onClick.AddListener(SelectVessel);
        }

        void SelectVessel()
        {
            TextContent = ScrollViewButtonGO.GetComponent<Text>().text;
            SelectionGO.GetComponent<Text>().text = TextContent;
            SelectedVessel = ScrollViewButtonGO.name; //in-game vessel reference
            SelectedVesselName = TextContent; //name of the vessel
            Debug.Log(SelectedVessel);
            Debug.Log(SelectedVesselName);
        }
    }
}