using UnityEngine;

namespace Interfaces
{
    public class CustomPlayerPrefs : IPlayerPrefs
    {
        public int GetInt(string key, int defaultValue = 0)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }

        public void SetInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }
    }
}