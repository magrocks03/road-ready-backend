using System;

namespace RoadReadyAPI.Exceptions
{
    public class IssueReportingException : Exception
    {
        public IssueReportingException(string message) : base(message)
        {
        }
    }
}