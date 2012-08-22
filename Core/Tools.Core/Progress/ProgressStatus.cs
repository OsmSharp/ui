using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Core.Progress
{
    /// <summary>
    /// Status object reporting progress information.
    /// </summary>
    public class ProgressStatus
    {
        private int _totalItemsNumber;
        public int TotalNumber
        {
            get { return _totalItemsNumber; }
            set { _totalItemsNumber = value; }
        }

        public string Message { get; set; }

        public enum ProgressStatusEnum
        {
            Failed,
            Querying,
            Succeeded,
            Down,
            Busy
        }

        private ProgressStatusEnum _status;
        public ProgressStatusEnum Status
        {
            get { return _status; }
            set { _status = value; }
        }

        private int _currentNumber;
        public int CurrentNumber
        {
            get { return _currentNumber; }
            set { _currentNumber = value; }
        }


        private object _ent;
        public object CurrentEntity
        {
            get { return _ent; }
            set { _ent = value; }
        }

        private DateTime _timeRemaining;
        public DateTime TimeRemaining
        {
            get { return _timeRemaining; }
            set { _timeRemaining = value; }
        }

        private DateTime _timePassed;
        public DateTime TimePassed
        {
            get { return _timePassed; }
            set { _timePassed = value; }
        }
        public string TimePassedString
        {
            get
            {
                string str = "";
                if (this.TimePassed.Day - 1 > 0)
                {
                    str = str + (this.TimePassed.Day - 1).ToString() + " days ";
                }
                if (this.TimePassed.TimeOfDay.Hours > 0)
                {
                    str = str + this.TimePassed.TimeOfDay.Hours + " hours ";
                }
                return str + this.TimePassed.TimeOfDay.Minutes + "min " + this.TimePassed.TimeOfDay.Seconds + "s";
            }
        }
        public string TimeRemainingString
        {
            get
            {
                string str = "";
                if (this.TimeRemaining.Day - 1 > 0)
                {
                    str = str + (this.TimeRemaining.Day - 1).ToString() + " days ";
                }
                if (this.TimeRemaining.TimeOfDay.Hours > 0)
                {
                    str = str + this.TimeRemaining.TimeOfDay.Hours + " hours ";
                }
                return str + this.TimeRemaining.TimeOfDay.Minutes + "min " + this.TimeRemaining.TimeOfDay.Seconds + "s";
            }
        }

        public double ProgressPercentage
        {
            get
            {
                if (this.TotalNumber > 0)
                {
                    double percentage =
                        (((double)this.CurrentNumber) / ((double)this.TotalNumber)) * 100.0;
                    if (percentage > 100)
                    {
                        percentage = 100;
                    }
                    else if (percentage < 0)
                    {
                        percentage = 0;
                    }
                    return percentage;
                }
                return 0;
            }
        }

    } 
}
