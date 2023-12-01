/*
Простой статический класс, упрощающий обработку исключений

Примеры использования
Exceptions.TryFilterCatch(host.Close, Exceptions.NotFatal,
                    ex => logger.Error("Исключение при закрытии хоста сервиса.", ex));

 private bool TryGetTpmProcess(out Process process) {
            process = null;
            try {
                process = Process.GetProcessById(App.TpmProcessId.GetValueOrDefault());
                return true;
            } catch (Exception ex) {
                if (ex.IsFatal())
                    throw;
                return false;
            }
        }

*/
namespace Ankom.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class AnkomFatalException : Exception
    {
        private const string sModuleNameFormat = "Module Name [{0}]: {1}";
        public AnkomFatalException() : base()
        {
        }
        public AnkomFatalException(string message) : base(message)
        {
        }
        public AnkomFatalException(string moduleName, string message) :
            base(String.Format(sModuleNameFormat, moduleName, message))
        {
        }
        public AnkomFatalException(string message, Exception ex) :
            base(message, ex)
        {
        }
        public AnkomFatalException(string moduleName, string message, Exception ex) :
            base(String.Format(sModuleNameFormat, moduleName, message), ex)
        {
        }
    }
    public static class Exceptions
    {
        private static readonly List<Type> fatalExceptions = new List<Type> {
            typeof (OutOfMemoryException),
            typeof (StackOverflowException),
            typeof (AnkomFatalException),
//Ещё типы исключений, которые всегда являются фатальными
        };

        public static string FullMessage(this Exception ex)
        {
            var builder = new StringBuilder();
            while (ex != null)
            {
                builder.AppendFormat("{0}{1}", ex, Environment.NewLine);
                ex = ex.InnerException;
            }
            return builder.ToString();
        }

        public static void TryFilterCatch(Action tryAction, Func<Exception, bool> isRecoverPossible, Action handlerAction)
        {
            try
            {
                tryAction();
            }
            catch (Exception ex)
            {
                if (!isRecoverPossible(ex)) throw;
                handlerAction();
            }
        }

        public static void TryFilterCatch(Action tryAction, Func<Exception, bool> isRecoverPossible, Action<Exception> handlerAction)
        {
            try
            {
                tryAction();
            }
            catch (Exception ex)
            {
                if (!isRecoverPossible(ex))
                    throw;
                handlerAction(ex);
            }
        }

        public static bool NotFatal(this Exception ex)
        {
            return fatalExceptions.All(curFatal => ex.GetType() != curFatal);
        }

        public static bool IsFatal(this Exception ex)
        {
            return !NotFatal(ex);
        }
    }
}
