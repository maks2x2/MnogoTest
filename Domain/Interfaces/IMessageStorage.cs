using Domain.Models;

namespace Domain.Interfaces;

public interface IMessageStorage
{
    void PushMessage(Message message);
    Message? PullMessage();
}
