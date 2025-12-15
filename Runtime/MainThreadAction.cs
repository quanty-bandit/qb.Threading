using qb.Pattern;
using System;
using System.Collections.Concurrent;
using System.Threading;
namespace qb.Threading
{
    public class MainThreadAction:MBSingleton<MainThreadAction>
    {
        public override bool IsPersistent => true;

        private Thread mainThread;
        private readonly ConcurrentQueue<Action> mainThreadActions = new ConcurrentQueue<Action>();

        protected override void Awake()
        {
            base.Awake();
            mainThread = Thread.CurrentThread;
        }

        public void Do(Action action)
        {
            if (IsOnMainThread)
                action();
            else
                mainThreadActions.Enqueue(action);
        }

        public bool IsOnMainThread
        {
            get
            {
                return Thread.CurrentThread == mainThread;
            }
        }

        private void Update()
        {
            while (mainThreadActions.TryDequeue(out var action))
            {
                action?.Invoke();
            }
        }
    }
}
