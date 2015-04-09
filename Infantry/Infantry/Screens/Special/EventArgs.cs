using System;
using System.Collections.Generic;

namespace Infantry.Screens
{
    /// <summary>
    /// Our completion event args with bool results
    /// </summary>
    public class OperationCompletedEvent : EventArgs
    {
        bool result;

        /// <summary>
        /// Returns passed/failed boolean
        /// </summary>
        public bool Result
        {
            get { return result; }
            set { result = value; }
        }

        /// <summary>
        /// Returns a reason
        /// </summary>
        public string Reason
        {
            get;
            set;
        }

        public OperationCompletedEvent(bool result)
        {
            this.Result = result;
        }
    }
    /*
    /// <summary>
    /// Operation completed event
    /// </summary>
    public class OperationCompletedEvent : EventArgs
    {
        IAsyncResult asyncResult;

        public IAsyncResult AsyncResult
        {
            get { return asyncResult; }
            set { asyncResult = value; }
        }

        public OperationCompletedEvent(IAsyncResult async)
        {
            this.asyncResult = async;
        }
    }
    */
    /// <summary>
    /// Our async callback completed event
    /// </summary>
    public class AsyncCallbackCompletedEvent : System.ComponentModel.AsyncCompletedEventArgs
    {
        public AsyncCallbackCompletedEvent(Exception error, bool cancelled)
            : base(error, cancelled, null)
        {
        }
    }
}
