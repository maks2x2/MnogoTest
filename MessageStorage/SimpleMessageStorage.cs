using System.Collections.Concurrent;
using Domain.Interfaces;
using Domain.Models;

namespace MessageStorage
{
    public class SimpleMessageStorage : IMessageStorage
    {
        private static readonly List<Message> _queue;
        private static Object _lockObject;

        static SimpleMessageStorage()
        {
            _queue = new List<Message>();
            _lockObject = new Object();
        }

        public SimpleMessageStorage()
        {

        }

        public void PushMessage(Message message)
        {
            lock (_lockObject)
            {
                _queue.Add(message);
            }
        }

        public Message? PullMessage()
        {
            lock (_lockObject)
            {

                var msg = _queue.MaxBy(x => x.Priority);
                if (msg != null)
                {
                    _queue.Remove(msg);
                    return msg;
                }

                return null;
            }
        }

    }
}