using JetBrains.Annotations;

namespace GZipTest
{
    [PublicAPI]
    public struct VeeamResult
    {
        public bool IsSuccess { get; }
        public VeeamError ErrorInfo { get; }

        public VeeamResult(bool isSuccess, VeeamError errorInfo)
        {
            IsSuccess = isSuccess;
            ErrorInfo = errorInfo;
        }

        public static implicit operator VeeamResult(VeeamError errorInfo) => new VeeamResult(isSuccess: false, errorInfo: errorInfo);

        public static implicit operator VeeamResult(VeeamWarning warningInfo) => new VeeamResult(isSuccess: false, errorInfo: warningInfo);

        public static implicit operator VeeamResult(bool isSuccess) => new VeeamResult(isSuccess: isSuccess, errorInfo: new VeeamError());

        public static implicit operator bool(VeeamResult data) => data.IsSuccess;
    }

    [PublicAPI]
    public struct VeeamResult<TErrorCode>
    {
        public bool IsSuccess { get; }
        public VeeamError<TErrorCode> ErrorInfo { get; }

        public VeeamResult(bool isSuccess, VeeamError<TErrorCode> errorInfo)
        {
            IsSuccess = isSuccess;
            ErrorInfo = errorInfo;
        }

        public static implicit operator VeeamResult<TErrorCode>(VeeamError<TErrorCode> errorInfo) =>
            new VeeamResult<TErrorCode>(isSuccess: false, errorInfo: errorInfo);

        public static implicit operator VeeamResult<TErrorCode>(VeeamWarning<TErrorCode> warningInfo) =>
            new VeeamResult<TErrorCode>(isSuccess: false, errorInfo: warningInfo);

        public static implicit operator VeeamResult<TErrorCode>(TErrorCode errorCode) =>
            new VeeamResult<TErrorCode>(isSuccess: false, errorInfo: errorCode!);

        public static implicit operator VeeamResult<TErrorCode>(bool isSuccess) =>
            new VeeamResult<TErrorCode>(isSuccess: isSuccess, errorInfo: new VeeamError<TErrorCode>());

        public static implicit operator VeeamResult(VeeamResult<TErrorCode> result) =>
            new VeeamResult(isSuccess: result.IsSuccess, errorInfo: result.ErrorInfo);

        public static implicit operator bool(VeeamResult<TErrorCode> data) => data.IsSuccess;
    }
}