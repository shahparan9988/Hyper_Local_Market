using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Shared.Results
{
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public Error Error { get; }
        protected Result(bool isSuccess, Error error) 
        {
            if(isSuccess && error != Error.None)
                throw new InvalidOperationException("Successful result cannot have an error.");

            if (!isSuccess && error == Error.None)
                throw new InvalidOperationException("Failed result must have an error.");

            this.IsSuccess = isSuccess;
            Error = error;

        }

        public static Result Success()
        {
            return new Result(true, Error.None);
        }

        public static Result Failure(Error error)
        {
            return new Result(false, error);
        }

        public static Result Failure(string code, string error)
        {
            return Failure(Error.Create(code, error));
        }


    }
}
