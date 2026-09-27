using UnityEngine;

namespace _Template.Core.EventSystem.Events.Application
{
    public class ApplicationBootEvent
    {
        public float BootTime;
        public ApplicationBootEvent(float bootTime)
        {
            this.BootTime = bootTime;

            Debug.Log("Boot Time: " + bootTime);
        }
    }
}
