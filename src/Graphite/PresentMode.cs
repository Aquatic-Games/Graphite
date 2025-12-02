namespace Graphite;

public enum PresentMode
{
    Immediate,
    Mailbox,
    Fifo,
    FifoRelaxed,
 
    /// <summary>
    /// Enables VSync. Tries present modes in this order: <see cref="Mailbox"/> -> <see cref="Fifo"/>
    /// </summary>
    VSyncOn,
    
    /// <summary>
    /// Disables VSync. Tries present modes in this order <see cref="Immediate"/> -> <see cref="Fifo"/>
    /// </summary>
    VSyncOff
}