using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Shared.Results
{
    public sealed class Result<T> : Result
    {
        private readonly T? _value;

        public T Value
        {
            get
            {
                if (IsFailure)
                    throw new InvalidOperationException("Cannot access the value of a failed result.");
                return _value!;
            }
        }

        private Result(T? value) : base(true, Error.None)
        {
            _value = value;
        } 
        
        private Result(Error error) : base(false, error)
        {
            _value = default;

        }

        public static Result<T> Success(T value)
        {
            return new Result<T>(value);
        }

        public static new Result<T> Failure(Error error)
        {
            return new Result<T>(error);
        }

        public static new Result<T> Failure(string code, string error)
        {
            return Failure(Error.Create(code, error));
        }

    }
}
