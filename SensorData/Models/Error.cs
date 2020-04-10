using System;

namespace SensorData.Models
{
    public class Error
    {
        public string Message { get; }

        public Error(string message)
        {
            Message = message;
        }

        public Error(Exception exception)
        {
            Message = exception.Message;
        }

    }
}
