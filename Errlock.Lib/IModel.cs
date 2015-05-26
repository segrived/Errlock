using System;
using LiteDB;

namespace Errlock.Lib
{
    public interface IModel
    {
        Guid Id { get; }
    }
}