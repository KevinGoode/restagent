using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kge
{
    namespace Agent
    {
        namespace Library
        {
            public static class Retry
            {
                public static void Do(Action action, TimeSpan retryInterval, int maxAttemptCount = 3)
                {
                    Do<object>(() =>
                    {
                        action();
                        return null;
                    }, retryInterval, maxAttemptCount);
                }

                public static T Do<T>(Func<T> action, TimeSpan retryInterval, int maxAttemptCount = 3)
                {
                    Exception latestException = null;

                    for (int attempted = 0; attempted < maxAttemptCount; attempted++)
                    {
                        try
                        {
                            if (attempted > 0)
                            {
                                Thread.Sleep(retryInterval);
                            }
                            return action();
                        }
                        catch (Exception ex)
                        {
                            latestException = ex;
                        }
                    }

                    throw latestException;
                }
            }
        }
    }
}