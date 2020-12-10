using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PartialViewInterface.Utils
{
    public class TaskHelper
    {
        public static Task Start(Action action)
        {
            return Task.Factory.StartNew(action, TaskHelper.CreationOptions);
        }

        public static Task StartNewThread(Action action)
        {
            return Task.Factory.StartNew(action, TaskCreationOptions.LongRunning);
        }

        public static Task Start<T>(Action<T> action, T userState)
        {
            object[] state = new object[]
            {
                action,
                userState
            };
            return Task.Factory.StartNew(delegate (object obj)
            {
                Action<T> action2 = (obj as object[])[0] as Action<T>;
                T obj2 = (T)((object)(obj as object[])[1]);
                action2(obj2);
            }, state, TaskHelper.CreationOptions);
        }

        public static Task Start<T1, T2>(Action<T1, T2> action, T1 userState1, T2 userState2)
        {
            object[] state = new object[]
            {
                action,
                userState1,
                userState2
            };
            return Task.Factory.StartNew(delegate (object obj)
            {
                Action<T1, T2> action2 = (obj as object[])[0] as Action<T1, T2>;
                T1 arg = (T1)((object)(obj as object[])[1]);
                T2 arg2 = (T2)((object)(obj as object[])[2]);
                action2(arg, arg2);
            }, state, TaskHelper.CreationOptions);
        }

        public static Task Start<T1, T2, T3>(Action<T1, T2, T3> action, T1 userState1, T2 userState2, T3 userState3)
        {
            object[] state = new object[]
            {
                action,
                userState1,
                userState2,
                userState3
            };
            return Task.Factory.StartNew(delegate (object obj)
            {
                Action<T1, T2, T3> action2 = (obj as object[])[0] as Action<T1, T2, T3>;
                T1 arg = (T1)((object)(obj as object[])[1]);
                T2 arg2 = (T2)((object)(obj as object[])[2]);
                T3 arg3 = (T3)((object)(obj as object[])[3]);
                action2(arg, arg2, arg3);
            }, state, TaskHelper.CreationOptions);
        }

        public static Task<R> Start<R>(Func<R> action)
        {
            return Task.Factory.StartNew<R>(action, TaskHelper.CreationOptions);
        }

        public static Task<R> Start<T, R>(Func<T, R> action, T userState)
        {
            object[] state = new object[]
            {
                action,
                userState
            };
            return Task.Factory.StartNew<R>(delegate (object obj)
            {
                Func<T, R> func = (obj as object[])[0] as Func<T, R>;
                T arg = (T)((object)(obj as object[])[1]);
                return func(arg);
            }, state, TaskHelper.CreationOptions);
        }

        public static Task<R> Start<T1, T2, R>(Func<T1, T2, R> action, T1 userState1, T2 userState2)
        {
            object[] state = new object[]
            {
                action,
                userState1,
                userState2
            };
            return Task.Factory.StartNew<R>(delegate (object obj)
            {
                Func<T1, T2, R> func = (obj as object[])[0] as Func<T1, T2, R>;
                T1 arg = (T1)((object)(obj as object[])[1]);
                T2 arg2 = (T2)((object)(obj as object[])[2]);
                return func(arg, arg2);
            }, state, TaskHelper.CreationOptions);
        }


        public static TaskCreationOptions CreationOptions
        {
            get
            {
                int num = 0;
                int num2 = 0;
                ThreadPool.GetAvailableThreads(out num, out num2);
                if (num >= 2)
                {
                    return TaskCreationOptions.PreferFairness;
                }
                return TaskCreationOptions.LongRunning;
            }
        }
    }
}
