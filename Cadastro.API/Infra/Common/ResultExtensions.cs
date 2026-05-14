namespace Cadastro.API.Infra.Common;

public static class ResultExtensions
{
    public static Result<T> Bind<T>(this Result result, Func<Result<T>> nextResult) =>
        result.IsSuccess ? nextResult() : Result<T>.Failure(result.Error);

    public static async Task<Result<T>> BindAsync<T>(
        this Task<Result> resultTask,
        Func<Task<Result<T>>> nextResult)
    {
        var result = await resultTask;
        return result.IsSuccess ? await nextResult() : Result<T>.Failure(result.Error);
    }

    public static async Task<Result<TNext>> BindAsync<T, TNext>(
        this Task<Result<T>> resultTask,
        Func<T, Task<Result<TNext>>> nextResult)
    {
        var result = await resultTask;
        return result.IsSuccess ? await nextResult(result.Value) : Result<TNext>.Failure(result.Error);
    }

    public static async Task<Result<TNext>> MapAsync<T, TNext>(
        this Task<Result<T>> resultTask,
        Func<T, Task<TNext>> transform)
    {
        var result = await resultTask;
        if (!result.IsSuccess)
            return Result<TNext>.Failure(result.Error);

        var value = await transform(result.Value);
        return Result<TNext>.Success(value);
    }

    public static async Task<Result<T>> TapAsync<T>(
        this Task<Result<T>> resultTask,
        Func<T, Task> action)
    {
        var result = await resultTask;
        if (result.IsSuccess)
            await action(result.Value);
        return result;
    }
}
