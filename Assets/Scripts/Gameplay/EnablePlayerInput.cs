using System;
using System.Diagnostics;
using Platformer.Core;
using Platformer.Model;

namespace Platformer.Gameplay
{
    /// <summary>
    /// This event is fired when user input should be enabled.
    /// </summary>
    public class EnablePlayerInput : Simulation.Event<EnablePlayerInput>
    {
        PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public override void Execute()
        {
            var player = model.player;
            var id = model.player.GetInstanceID();
            Console.WriteLine("Hello mum");
            player.controlEnabled = true;
        }
    }
}