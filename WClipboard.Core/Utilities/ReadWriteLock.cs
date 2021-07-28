using System;
using System.Collections.Generic;
using System.Threading;

namespace WClipboard.Core.Utilities
{
    public class ReadWriteLock
    {
        private int writeLock;
        private int? writeThreadId;
        private int readCounter;
        private readonly List<int> readThreadIds;

        public ReadWriteLock()
        {
            writeLock = 0;
            writeThreadId = null;
            readCounter = 0;
            readThreadIds = new List<int>();
        }

        public void EnterRead() => EnterReadForThread(Thread.CurrentThread);

        public void EnterReadForThread(Thread t)
        {
            int threadId = t.ManagedThreadId;
            if (writeThreadId != threadId)
            {
                SpinWait.SpinUntil(() => writeLock == 0);
            }

            Interlocked.Increment(ref readCounter);
            lock (readThreadIds)
            {
                readThreadIds.Add(threadId);
            }
        }

        public void ExitRead()
        {
            int currentThreadId = Thread.CurrentThread.ManagedThreadId;
            if (Interlocked.Decrement(ref readCounter) < 0)
            {
                throw new InvalidOperationException("Read is exited to many times");
            }
            lock(readThreadIds)
            {
                if(!readThreadIds.Remove(currentThreadId))
                {
                    throw new InvalidOperationException("Read is exited to many times for this thread");
                }
            }
        }

        public void ExitWrite()
        {
            int currentThreadId = Thread.CurrentThread.ManagedThreadId;
            if (currentThreadId != writeThreadId) //No worry about raceconditions since it can only be true if set by this thread, so not async
            {
                throw new InvalidOperationException("You can only exit write on the thread that entered it");
            }

            if (writeLock == 1) // will become zero
            {
                writeThreadId = null; //First set to null, then free lock
            }
            if (Interlocked.Decrement(ref writeLock) < 0)
            {
                throw new InvalidOperationException("Write is exited to many times for this thread");
            }
        }

        public void EnterWrite()
        {
            int currentThreadId = Thread.CurrentThread.ManagedThreadId;

            if (writeThreadId == currentThreadId) //No worry about raceconditions since it can only be true if set by this thread, so not async
            {
                Interlocked.Increment(ref writeLock);
            }
            else
            {
                lock (readThreadIds)
                {
                    if (readThreadIds.Contains(currentThreadId))
                    {
                        throw new InvalidOperationException("Could not enter write since a read on this thread is still in progress");
                    }
                }
                //We are on this thread now, so it cannot allocate new reads

                for (; ; )
                {
                    SpinWait.SpinUntil(() => readCounter == 0);
                    SpinWait.SpinUntil(() => Interlocked.CompareExchange(ref writeLock, 1, 0) == 0); //Wait for write lock
                    if (readCounter == 0) //Check if readCounter is still 0
                        break;
                    //Read counter was not 0 anymore so free lock again
                    Interlocked.Exchange(ref writeLock, 0);
                }
                
                writeThreadId = currentThreadId;
            }
        }

        public void ExchangeWriteForRead()
        {
            int currentThreadId = Thread.CurrentThread.ManagedThreadId;
            if (writeThreadId != currentThreadId)
            {
                throw new InvalidOperationException("Thread has no write lock");
            } 
            else if(writeLock != 1)
            {
                throw new InvalidOperationException("Thread must only have 1 write lock left in order to exchange");
            }

            //Enter Read
            Interlocked.Increment(ref readCounter);
            lock (readThreadIds)
            {
                readThreadIds.Add(currentThreadId);
            }

            ExitWrite();
        }
    }
}
