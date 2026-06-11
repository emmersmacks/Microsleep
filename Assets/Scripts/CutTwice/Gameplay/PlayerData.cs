using UnityEngine;
using UnityEngine.Events;

namespace CutTwice.Gameplay
{
    // TODO: Refactor This
    public static class PlayerData
    {
        private static string steeringWheelId = null;
        
        public static string SteeringWheelId
        {
            get => steeringWheelId;
            set
            {
                steeringWheelId = value;
                OnSteeringWheelChanged?.Invoke();
            }
        }

        public static UnityEvent OnSteeringWheelChanged = new (); 

        public static void Save()
        {
            PlayerPrefs.SetString("SteeringWheelId", SteeringWheelId);
        }
        
        public static void Load()
        {
            SteeringWheelId = PlayerPrefs.GetString("SteeringWheelId");
        }
    }
}