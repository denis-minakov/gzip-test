using JetBrains.Annotations;

namespace GZipTest
{
    [PublicAPI]
    public struct VeeamDataResult<TData> where TData : class
    {
        public bool HasValue { get; }
        public TData? Value { get; }
        public VeeamError ErrorInfo { get; }

        public VeeamDataResult(TData? value, VeeamError errorInfo = new VeeamError())
        {
            HasValue = value != null;
            Value = value;
            ErrorInfo = errorInfo;
        }

        public VeeamDataResult(VeeamError errorInfo)
            : this(value: null, errorInfo: errorInfo)
        {
        }

        public TData? GetValueOrDefault() => HasValue ? Value : null;

        public TData GetValueOrDefault([NotNull] TData defaultData) => HasValue ? (Value ?? defaultData) : defaultData;

        public static implicit operator VeeamDataResult<TData>(VeeamError errorInfo) =>
            new VeeamDataResult<TData>(value: null, errorInfo: errorInfo);

        public static implicit operator VeeamDataResult<TData>(VeeamWarning errorInfo) =>
            new VeeamDataResult<TData>(value: null, errorInfo: errorInfo);

        public static implicit operator VeeamDataResult<TData>(TData data) => new VeeamDataResult<TData>(value: data);

        public static implicit operator TData?(VeeamDataResult<TData> data) => data.HasValue ? data.Value : null;

        public static implicit operator bool(VeeamDataResult<TData> data) => data.HasValue;

        public static implicit operator VeeamResult(VeeamDataResult<TData> data) =>
            data.HasValue ? new VeeamResult(isSuccess: true, errorInfo: new VeeamError()) : data.ErrorInfo;
    }

    [PublicAPI]
    public struct VeeamDataResult<TData, TErrorCode> where TData : class
    {
        public bool HasValue { get; }
        public TData? Value { get; }
        public VeeamError<TErrorCode> ErrorInfo { get; }

        public VeeamDataResult(TData? value, VeeamError<TErrorCode> errorInfo = new VeeamError<TErrorCode>())
        {
            HasValue = value != null;
            Value = value;
            ErrorInfo = errorInfo;
        }

        public VeeamDataResult(VeeamError<TErrorCode> errorInfo)
            : this(value: null, errorInfo: errorInfo)
        {
        }

        public TData? GetValueOrDefault() => HasValue ? Value : null;

        public TData GetValueOrDefault([NotNull] TData defaultData) => HasValue ? (Value ?? defaultData) : defaultData;

        public static implicit operator VeeamDataResult<TData, TErrorCode>(VeeamError<TErrorCode> errorInfo) =>
            new VeeamDataResult<TData, TErrorCode>(errorInfo: errorInfo);

        public static implicit operator VeeamDataResult<TData, TErrorCode>(TErrorCode errorCode) =>
            new VeeamDataResult<TData, TErrorCode>(errorInfo: errorCode!);

        public static implicit operator VeeamDataResult<TData, TErrorCode>(TData data) => new VeeamDataResult<TData, TErrorCode>(value: data);

        public static implicit operator TData?(VeeamDataResult<TData, TErrorCode> data) => data.HasValue ? data.Value : null;

        public static implicit operator bool(VeeamDataResult<TData, TErrorCode> data) => data.HasValue;

        public static implicit operator VeeamResult<TErrorCode>(VeeamDataResult<TData, TErrorCode> data) =>
            data.HasValue
                ? new VeeamResult<TErrorCode>(isSuccess: true, errorInfo: new VeeamError<TErrorCode>())
                : data.ErrorInfo;
    }
}