namespace Interfaces
{
    public interface IPlayerPrefs
    {
        int GetInt(string key, int defaultValue = 0);
        void SetInt(string key, int value);
    }
}