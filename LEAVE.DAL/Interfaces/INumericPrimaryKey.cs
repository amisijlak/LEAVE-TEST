using System;
using System.Collections.Generic;
using System.Text;

namespace LEAVE.DAL
{
    public interface INumericPrimaryKey : IPrimaryKeyEnabled<long>
    {
    }

    public interface IPrimaryKeyEnabled<T>: ILEAVEModel
    {
        T Id { get; set; }
    }

    public interface ILEAVEModel
    {

    }
}
