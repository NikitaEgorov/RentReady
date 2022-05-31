using JetBrains.Annotations;

namespace TimeEntryFunction.Models;

[PublicAPI]
public class EntityRequest
{
    // Swagger doesn't support DateOnly :(
    public DateTime StartOn { get; set; }

    public DateTime EndOn { get; set; }

    public void Validate()
    {
        if (this.StartOn.TimeOfDay != TimeSpan.Zero || this.EndOn.TimeOfDay != TimeSpan.Zero)
        {
            throw new ArgumentException(
                $"{nameof(this.StartOn)} {this.StartOn} and {nameof(this.EndOn)} {this.EndOn} should represent date only");
        }
    }
}
