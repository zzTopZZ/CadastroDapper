namespace Cadastro.API.Infra.Common;

public class Result
{
    public bool IsSuccess { get; }
    public string Error { get; }

    protected Result(bool isSuccess, string error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, null!);
    public static Result Failure(string error) => new(false, error);

    public TResult Match<TResult>(Func<TResult> onSuccess, Func<string, TResult> onFailure) =>
        IsSuccess ? onSuccess() : onFailure(Error);

    public void Match(Action onSuccess, Action<string> onFailure)
    {
        if (IsSuccess) onSuccess();
        else onFailure(Error);
    }
}

public class Result<T>
{
    public bool IsSuccess { get; }
    public T Value { get; }
    public string Error { get; }

    protected Result(bool isSuccess, T value, string error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public static Result<T> Success(T value) => new(true, value, null!);
    public static Result<T> Failure(string error) => new(false, default!, error);

    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<string, TResult> onFailure) =>
        IsSuccess ? onSuccess(Value) : onFailure(Error);

    public void Match(Action<T> onSuccess, Action<string> onFailure)
    {
        if (IsSuccess) onSuccess(Value);
        else onFailure(Error);
    }

    public Result<TNext> Bind<TNext>(Func<T, Result<TNext>> nextResult) =>
        IsSuccess ? nextResult(Value) : Result<TNext>.Failure(Error);

    public Result<TNext> Map<TNext>(Func<T, TNext> transform) =>
        IsSuccess ? Result<TNext>.Success(transform(Value)) : Result<TNext>.Failure(Error);

    public Result<T> Tap(Action<T> action)
    {
        if (IsSuccess) action(Value);
        return this;
    }

    public Result<T> TapError(Action<string> action)
    {
        if (!IsSuccess) action(Error);
        return this;
    }

    public T GetValueOrThrow() =>
        IsSuccess ? Value : throw new InvalidOperationException($"Result failed: {Error}");

    public T GetValueOrDefault(T defaultValue) =>
        IsSuccess ? Value : defaultValue;
}
