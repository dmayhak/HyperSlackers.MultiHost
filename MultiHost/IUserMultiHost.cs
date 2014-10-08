using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSlackers.MultiHost
{
    public interface IUserMultiHost<out TKey, THostKey> : IUser<TKey>
        where TKey : IEquatable<TKey>
        where THostKey : IEquatable<THostKey>
    {
        THostKey HostId { get; set; }
    }

    public interface IMultiHostUserString : IUserMultiHost<string, string>
    {
        
    }

    public interface IMultiHostUserGuid : IUserMultiHost<Guid, Guid>
    {

    }

    public interface IMultiHostUserInt : IUserMultiHost<int, int>
    {

    }

    public interface IMultiHostUserLong : IUserMultiHost<long, long>
    {

    }
}
