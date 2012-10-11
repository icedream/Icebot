using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Icebot
{
    public partial class Bot
    {
        internal ManualResetEventSlim _asyncReadingLock = new ManualResetEventSlim(true);
        internal ManualResetEventSlim _asyncWritingLock = new ManualResetEventSlim(true);
        private CancellationTokenSource _asyncCancel = new CancellationTokenSource();

        private void _asyncReadingProcedure()
        {
            try
            {
                _log.Debug("Asynchronous reading loop started (this is the asyncReadingProcedure thread)");

                while (true)
                {
                    try
                    {
                        // Wait for the async event to be set
                        _asyncReadingLock.Wait();

                        lock (_asyncReadingLock)
                        {
                            // Read from stream
                            string recv;
                            //while (_socket.Available == 0)
                            //    Thread.Sleep(25);
                            recv = _reader.ReadLine();
                            _log.DebugFormat("RECV: {0}", recv);

                            // Asynchronously handle the line
                            _taskfactory.StartNew(() => _lineHandler(recv), _asyncCancel.Token);
                        }
                    }
                    catch (Exception eng)
                    {
                        _log.Error("Error in reading thread", eng);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _log.Debug("Asynchronous reading loop shutting down due to internal request");
            }
        }

        private void _asyncWritingProcedure()
        {
            try
            {
                _log.Debug("Asynchronous writing loop started (this is the asyncWritingProcedure thread)");

                while (true)
                {
                    try
                    {
                        // Wait for the queue to be filled
                        while ((from m in _messageQueue select m.Count).Sum() == 0)
                        {
                            _asyncWritingLock.Wait();
                            Thread.Sleep(20);
                        }

                        lock (_asyncQueueLock)
                        {
                            // Wait for the async event to be set
                            _asyncWritingLock.Wait();

                            ushort msg = 0;

                            // This should emulate the anti-flood of the ZNC bouncer
                            lock (_asyncWritingLock)
                            {
                                for (int i = _messageQueue.Length - 1; i >= 0; i--)
                                    while (_messageQueue[i].Count > 0)
                                    {
                                        string line = _messageQueue[i].Dequeue();
                                        // TODO: Count timing of the event invocation and subtract it from total wait time
                                        _writer.WriteLine(line);
                                        _writer.Flush();
                                        OnRawLineSent(line);
                                        if (msg == QueueIntervalAfterMessages)
                                            Thread.Sleep(this.QueueInterval);
                                        else
                                            msg++;
                                    }
                            }
                            _asyncQueueLock.Reset();
                        }
                    }
                    catch (Exception eng)
                    {
                        _log.Error("Error in writing thread", eng);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _log.Debug("Asynchronous writing loop shutting down due to internal request");
            }
        }
    }
}
