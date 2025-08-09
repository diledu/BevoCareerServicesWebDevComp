namespace Group_11.Utilities
{
    // instantiate global datetime "Now" variable
    public interface SysDate
    {
        DateTime currDateTime { get; }
        void SetDateTime(DateTime newTime);
    }

    // global class for system date
    public class CustomDateTimeProvider : SysDate
    {
        private DateTime _currentDateTime;

        public CustomDateTimeProvider()
        {
            // default to current time
            _currentDateTime = DateTime.Now;
        }

        public DateTime currDateTime => _currentDateTime;

        public void SetDateTime(DateTime newDateTime)
        {
            _currentDateTime = newDateTime;
        }
    }
}
