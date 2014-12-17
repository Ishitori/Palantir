namespace Ix.Palantir.Queueing.API.Command
{
    using System;
    using System.Collections.Generic;

    public interface ICommandMessage
    {
        string CommandName { get; }
        DateTime SendingDate { get; set; }
        int TryIndex { get; set; }
        int TtlInMinutes { get; set; }
        bool IsCorrupted();
        void OnAfterDeserialization();
        void MarkAsCompleted();
        IDictionary<string, string> GetProperties();
    }
}