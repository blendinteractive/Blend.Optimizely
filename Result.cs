using System;
using System.Diagnostics.CodeAnalysis;

namespace Blend.Optimizely
{
    public record Result<TValue, TError>(TValue? Value, TError? Error, bool IsSuccessful)
    {
#pragma warning disable CA2225 // Operator overloads have named alternates
        public static implicit operator Result<TValue, TError>(TValue value) => new Result<TValue, TError>(value, default, true);

        public static implicit operator Result<TValue, TError>(TError error) => new Result<TValue, TError>(default, error, true);
#pragma warning restore CA2225 // Operator overloads have named alternates

        public bool HasSuccessValue([NotNullWhen(true)] out TValue? value)
        {
            if (IsSuccessful && Value is not null)
            {
                value = Value;
                return true;
            }

            value = default;
            return false;
        }

        public bool HasError([NotNullWhen(true)] out TError? error)
        {
            if (!IsSuccessful && Error is not null)
            {
                error = Error;
                return true;
            }

            error = default;
            return false;
        }
    }

    public static class Result
    {
        public static Result<TValue, TError> Success<TValue, TError>(TValue value) => new Result<TValue, TError>(value, default, true);
        public static Result<TValue, TError> Error<TValue, TError>(TError error) => new Result<TValue, TError>(default, error, false);
    }

    public static class ResultExtensions
    {
        public static TValue? Assume<TValue, TError>(this Result<TValue, TError> result, Func<TError?, string>? getMessage = null)
        {
            if (result.IsSuccessful)
                return result.Value;

            string? errorMessage = getMessage != null ? getMessage(result.Error) :
                (result.Error is not null ? result.Error.ToString() : "NULL ERROR");

            throw new InvalidOperationException(errorMessage);
        }

        public static void Switch<TValue, TError>(this Result<TValue, TError> result, Action<TError> onError, Action<TValue> onSuccess)
        {
            if (result.IsSuccessful && result.Value is not null)
                onSuccess(result.Value);
            else if (!result.IsSuccessful && result.Error is not null)
                onError(result.Error);
            else
                throw new NotImplementedException("Switch does not handle null values or errors");
        }

        public static TOut Match<TValue, TError, TOut>(this Result<TValue, TError> result, Func<TError, TOut> onError, Func<TValue, TOut> onSuccess)
        {
            if (result.IsSuccessful && result.Value is not null)
                return onSuccess(result.Value);
            else if (!result.IsSuccessful && result.Error is not null)
                return onError(result.Error);
            else
                throw new NotImplementedException("Match does not handle null values or errors");
        }
    }
}
