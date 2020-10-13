using System;
using JetBrains.Annotations;

namespace GZipTest
{
    [PublicAPI]
    public struct VeeamError
    {
        public VeeamErrorLevel ErrorLevel { get; }
        public string? ErrorText { get; }
        public Exception? Exception { get; }

        public VeeamError(VeeamErrorLevel errorLevel, string? errorText, Exception? exception = null)
        {
            ErrorLevel = errorLevel;
            ErrorText = errorText;
            Exception = exception;
        }

        public VeeamError(string errorText, Exception? exception = null)
            : this(errorLevel: VeeamErrorLevel.Error, errorText: errorText, exception: exception)
        {
        }

        [Pure]
        public VeeamError AttachPrefix(string prefix) => new VeeamError(
            errorLevel: ErrorLevel,
            errorText: string.Concat(str0: (prefix).Trim(), str1: " ", str2: (ErrorText ?? string.Empty)),
            exception: Exception);

        [Pure]
        public VeeamError<TErrorCode> AttachPrefix<TErrorCode>(TErrorCode errorCode, string? prefix) =>
            new VeeamError<TErrorCode>(
                errorLevel: ErrorLevel,
                errorCode: errorCode,
                errorText: string.Concat(str0: (prefix ?? string.Empty).Trim(), str1: " ", str2: (ErrorText ?? string.Empty)),
                exception: Exception);

        [Pure]
        public VeeamError<TErrorCode> WithErrorCode<TErrorCode>(TErrorCode errorCode) =>
            new VeeamError<TErrorCode>(errorLevel: ErrorLevel, errorCode: errorCode, errorText: ErrorText, exception: Exception);
    }

    [PublicAPI]
    public struct VeeamError<TErrorCode>
    {
        public VeeamErrorLevel ErrorLevel { get; }
        public TErrorCode ErrorCode { get; }
        public string? ErrorText { get; }
        public Exception? Exception { get; }

        public VeeamError(
            VeeamErrorLevel errorLevel,
            TErrorCode errorCode,
            string? errorText = null,
            Exception? exception = null)
        {
            ErrorLevel = errorLevel;
            ErrorCode = errorCode;
            ErrorText = errorText;
            Exception = exception;
        }

        public VeeamError(TErrorCode errorCode, string? errorText = null, Exception? exception = null)
            : this(errorLevel: VeeamErrorLevel.Error, errorCode: errorCode, errorText: errorText, exception: exception)
        {
            ErrorCode = errorCode;
        }

        [Pure]
        public VeeamError<TErrorCode> AttachPrefix(string prefix) => new VeeamError<TErrorCode>(
            errorLevel: ErrorLevel,
            errorCode: ErrorCode,
            errorText: string.Concat(str0: prefix, str1: " ", str2: ErrorText),
            exception: Exception);

        public static implicit operator VeeamError<TErrorCode>(TErrorCode errorCode) => new VeeamError<TErrorCode>(errorCode: errorCode);

        public static implicit operator VeeamError(VeeamError<TErrorCode> genericError) => new VeeamError(
            errorLevel: genericError.ErrorLevel,
            errorText: genericError.ErrorText,
            exception: genericError.Exception);

        [Pure]
        public VeeamError<TOtherErrorCode> Convert<TOtherErrorCode>([NotNull] Func<TErrorCode, TOtherErrorCode> convertFunc) =>
            new VeeamError<TOtherErrorCode>(
                errorLevel: ErrorLevel,
                errorCode: convertFunc(arg: ErrorCode),
                errorText: ErrorText,
                exception: Exception);

        [Pure]
        public VeeamError<TOtherErrorCode> WithErrorCode<TOtherErrorCode>(TOtherErrorCode errorCode) =>
            new VeeamError<TOtherErrorCode>(errorLevel: ErrorLevel, errorCode: errorCode, errorText: ErrorText, exception: Exception);
    }

    [PublicAPI]
    public struct VeeamWarning
    {
        public string? ErrorText { get; }
        public Exception? Exception { get; }

        public VeeamWarning(string? errorText, Exception? exception = null)
        {
            ErrorText = errorText;
            Exception = exception;
        }

        [Pure]
        public VeeamWarning AttachPrefix(string prefix) => new VeeamWarning(
            errorText: string.Concat(str0: prefix, str1: " ", str2: ErrorText),
            exception: Exception);

        public static implicit operator VeeamError(VeeamWarning warning) =>
            new VeeamError(errorLevel: VeeamErrorLevel.Warning, errorText: warning.ErrorText, exception: warning.Exception);

        public static implicit operator VeeamWarning(VeeamError error) =>
            new VeeamWarning(errorText: error.ErrorText, exception: error.Exception);
    }

    [PublicAPI]
    public struct VeeamWarning<TErrorCode>
    {
        public TErrorCode ErrorCode { get; }
        public string? ErrorText { get; }
        public Exception? Exception { get; }

        public VeeamWarning(TErrorCode errorCode, string? errorText = null, Exception? exception = null)
        {
            ErrorCode = errorCode;
            ErrorText = errorText;
            Exception = exception;
        }

        public VeeamWarning<TErrorCode> AttachPrefix(string prefix) => new VeeamWarning<TErrorCode>(
            errorCode: ErrorCode,
            errorText: string.Concat(str0: prefix, str1: " ", str2: ErrorText),
            exception: Exception);

        public static implicit operator VeeamWarning<TErrorCode>(TErrorCode errorCode) =>
            new VeeamWarning<TErrorCode>(errorCode: errorCode);

        public static implicit operator VeeamError<TErrorCode>(VeeamWarning<TErrorCode> warning) => new VeeamError<TErrorCode>(
            errorLevel: VeeamErrorLevel.Warning,
            errorCode: warning.ErrorCode,
            errorText: warning.ErrorText,
            exception: warning.Exception);

        public static implicit operator VeeamWarning<TErrorCode>(VeeamError<TErrorCode> error) =>
            new VeeamWarning<TErrorCode>(errorCode: error.ErrorCode, errorText: error.ErrorText, exception: error.Exception);

        [Pure]
        public VeeamWarning<TOtherErrorCode> Convert<TOtherErrorCode>([NotNull] Func<TErrorCode, TOtherErrorCode> convertFunc) =>
            new VeeamWarning<TOtherErrorCode>(errorCode: convertFunc(arg: ErrorCode), errorText: ErrorText, exception: Exception);
    }
}