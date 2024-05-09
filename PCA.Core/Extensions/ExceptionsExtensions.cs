namespace PCA.Core.Extensions;

public static class ExceptionsExtensions
{
    public static string Trace(this Exception? exception, bool includeStack = false)
    {
        if (exception == null)
        {
            return string.Empty;
        }

        StringBuilder stringBuilder = new StringBuilder();
        Exception ex = exception;
        StringBuilder stringBuilder2 = stringBuilder;
        StringBuilder stringBuilder3 = stringBuilder2;
        StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(10, 1, stringBuilder2);
        handler.AppendLiteral("Type: [");
        handler.AppendFormatted(exception.GetType());
        handler.AppendLiteral("]. ");
        stringBuilder3.Append(ref handler);
        stringBuilder2 = stringBuilder;
        StringBuilder stringBuilder4 = stringBuilder2;
        handler = new StringBuilder.AppendInterpolatedStringHandler(29, 3, stringBuilder2);
        handler.AppendLiteral("Message: ");
        handler.AppendFormatted(exception.Message);
        handler.AppendLiteral(", target: ");
        handler.AppendFormatted(exception.TargetSite);
        handler.AppendLiteral(", source: ");
        handler.AppendFormatted(exception.Source);
        stringBuilder4.Append(ref handler);
        if (includeStack)
        {
            stringBuilder2 = stringBuilder;
            StringBuilder stringBuilder5 = stringBuilder2;
            handler = new StringBuilder.AppendInterpolatedStringHandler(3, 1, stringBuilder2);
            handler.AppendLiteral(", ");
            handler.AppendFormatted(exception.StackTrace);
            handler.AppendLiteral(".");
            stringBuilder5.Append(ref handler);
        }

        while (exception != null)
        {
            if (ex != exception)
            {
                stringBuilder2 = stringBuilder;
                StringBuilder stringBuilder6 = stringBuilder2;
                handler = new StringBuilder.AppendInterpolatedStringHandler(10, 1, stringBuilder2);
                handler.AppendLiteral("Type: [");
                handler.AppendFormatted(exception.GetType());
                handler.AppendLiteral("]. ");
                stringBuilder6.Append(ref handler);
                stringBuilder2 = stringBuilder;
                StringBuilder stringBuilder7 = stringBuilder2;
                handler = new StringBuilder.AppendInterpolatedStringHandler(29, 3, stringBuilder2);
                handler.AppendLiteral("Message: ");
                handler.AppendFormatted(exception.Message);
                handler.AppendLiteral(", target: ");
                handler.AppendFormatted(exception.TargetSite);
                handler.AppendLiteral(", source: ");
                handler.AppendFormatted(exception.Source);
                stringBuilder7.Append(ref handler);
                if (includeStack)
                {
                    stringBuilder2 = stringBuilder;
                    StringBuilder stringBuilder8 = stringBuilder2;
                    handler = new StringBuilder.AppendInterpolatedStringHandler(3, 1, stringBuilder2);
                    handler.AppendLiteral(", ");
                    handler.AppendFormatted(exception.StackTrace);
                    handler.AppendLiteral(".");
                    stringBuilder8.Append(ref handler);
                }

                ex = exception;
            }

            if (exception.InnerException != null)
            {
                exception = exception.InnerException;
            }

            if (exception.InnerException == null && ex == exception)
            {
                exception = null;
            }
        }

        return stringBuilder.ToString();
    }
}
