namespace ContactBookAPI.Application.Common.Models;

public class Result
{
    private readonly List<string> errors;

    internal Result(bool succeeded, List<string> errors)
    {
        this.Succeeded = succeeded;
        this.errors = errors;
    }

    internal Result(
        bool succeeded,
        List<string> errors,
        string message = "",
        string messageDanger = "")
    {
        this.Succeeded = succeeded;
        this.errors = errors;

        this.Message = message;
        this.MessageDanger = messageDanger;
    }

    public bool Succeeded { get; }

    public string? Message { get; private set; }
    public string? MessageDanger { get; private set; }

    public List<string> Errors
        => this.Succeeded
            ? new List<string>()
            : this.errors;

    public void UpdateMessage(string message)
    {
        this.Message = message;
    }

    public void UpdateMessageDanger(string message)
    {
        this.MessageDanger = message;
    }

    public static Result Success
        => new Result(true, new List<string>());

    public static Result Failure(IEnumerable<string> errors)
        => new Result(false, errors.ToList());

    public static Result SuccessWithMessages(string message = "", string messageDanger = "")
        => new Result(true, new List<string>(), message, messageDanger);

    public static Result FailureWithMessages(string message = "", string messageDanger = "")
        => new Result(false, new List<string>(), message, messageDanger);

    public static implicit operator Result(string error)
        => Failure(new List<string> { error });

    public static implicit operator Result(List<string> errors)
        => Failure(errors.ToList());

    public static implicit operator Result(bool success)
        => success ? Success : Failure(new[] { "Unsuccessful operation." });

    public static implicit operator bool(Result result)
        => result.Succeeded;
}

public class Result<TData> : Result
{
    private readonly TData data;

    private Result(bool succeeded, TData data, List<string> errors)
        : base(succeeded, errors)
    {
        this.data = data;
    }

    private Result(
        bool succeeded,
        TData data,
        List<string> errors,
        string message = "",
        string messageDanger = "")
        : base(succeeded, errors, message, messageDanger)
    {
        this.data = data;
    }

    public TData Data
        => this.Succeeded
            ? this.data
            : throw new InvalidOperationException(
                $"{nameof(this.Data)} is not available with a failed result. Use {this.Errors} instead.");

    public new static Result<TData> Success(TData data)
        => new Result<TData>(true, data, new List<string>());

    public new static Result<TData> Failure(IEnumerable<string> errors)
        => new Result<TData>(false, default!, errors.ToList());

    public static Result<TData> SuccessWithMessages(TData data = default!, string message = "", string messageDanger = "")
       => new Result<TData>(true, data, new List<string>(), message, messageDanger);

    public static Result<TData> FailureWithMessages(IEnumerable<string> errors, string message = "", string messageDanger = "")
        => new Result<TData>(false, data: default!, errors.ToList(), message, messageDanger);

    public static implicit operator Result<TData>(string error)
        => Failure(new List<string> { error });

    public static implicit operator Result<TData>(List<string> errors)
        => Failure(errors);

    public static implicit operator Result<TData>(TData data)
        => Success(data);

    public static implicit operator bool(Result<TData> result)
        => result.Succeeded;
}
