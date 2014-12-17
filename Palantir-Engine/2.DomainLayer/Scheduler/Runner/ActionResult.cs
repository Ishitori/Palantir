namespace Ix.Palantir.Scheduler.Runner
{
    using System;

    public class ActionResult
    {
        public ActionResult()
        {
            this.mState = ActionState.Completed;
            this.mException = null;
            this.mResult = null;
        }

        internal void SetActionState(ActionState state)
        {
            this.mState = state;
        }
        internal void SetException(Exception ex)
        {
            this.mException = ex;
        }
        internal void SetResult(object result)
        {
            this.mResult = result;
        }

        public Exception Exception
        {
            get
            {
                return this.mException;
            }
        }
        public object Result
        {
            get
            {
                return this.mResult;
            }
        }
        public ActionState State
        {
            get
            {
                return this.mState;
            }
        }

        private Exception mException;
        private object mResult;
        private ActionState mState;
    }
}