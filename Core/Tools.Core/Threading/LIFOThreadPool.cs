//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;

//namespace Tools.Core.Threading
//{
//    public class LIFOThreadPool
//    {
//        private int _max_queue_size;

//        #region Queue List

//        private List<QueuedThreadStart> _schedules_threads;

//        public void Queue(WaitCallback callBack,object param)
//        {
//            lock(_schedules_threads)
//            {
//                _schedules_threads.Add(
//                    new QueuedThreadStart(
//                        callBack,
//                        param));

                
//            }
//        }

//        private class QueuedThreadStart
//        {
//            public QueuedThreadStart(
//                WaitCallback call_back,
//                object param)
//            {
//                this.WaitCallBack = call_back;
//                this.Param = param;
//            }

//            public WaitCallback WaitCallBack { get; private set; }

//            public object Param { get; private set; }
//        }

//        #endregion

//        public void SetMaxQueueSize(int maxQueueSize)
//        {

//        }

//        public void SetMaxThreads(int workerThreads)
//        {

//        }

//        public QueueWorkerItem(WaitCallback callback,object param)
//        {

//        }


//    }
//}
