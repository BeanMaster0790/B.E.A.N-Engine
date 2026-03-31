using System.Text;

namespace Bean.Debug
{
    [Serializable]
    public class DebugFile
    {
        public List<string> LoggedValues;

        public List<string> LoggedValuesTimes;

        public List<string> MonitoredValues;

        public List<string> MonitoredValuesNames;
    }
}
