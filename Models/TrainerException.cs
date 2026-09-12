using System;
using System.Runtime.Serialization;

namespace MGSPW_MC_Cheat_Trainer.Models;

public class TrainerException : Exception
{
    public TrainerException()
    {
    }

    protected TrainerException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public TrainerException(string? message) : base(message)
    {
    }

    public TrainerException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}