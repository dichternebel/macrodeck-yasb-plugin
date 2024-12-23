using System.Text.Json;
using SuchByte.MacroDeck.ActionButton;
using SuchByte.MacroDeck.GUI;
using SuchByte.MacroDeck.GUI.CustomControls;
using SuchByte.MacroDeck.Plugins;

namespace dichternebel.YaSB.MacroDeckPlug
{
    public class YaSBAction : PluginAction
    {
        public override string Name => "DoAction";

        public override string Description => "Configure Streamer.Bot action to be triggerd.";

        public override bool CanConfigure => true;

        public override ActionConfigControl GetActionConfigControl(ActionConfigurator actionConfigurator)
        {
            return new DoActionControl(this, actionConfigurator);
        }

        public override void Trigger(string clientId, ActionButton actionButton)
        {
            var config = JsonSerializer.Deserialize<ActionConfiguration>(Configuration);
            if (config == null || string.IsNullOrWhiteSpace(config.streamerBotActionId) || string.IsNullOrWhiteSpace(config.streamerBotActionName)) return;
            WebSocketClient.Instance.SendMessageAsync(config.streamerBotActionId.ToString(), config.streamerBotActionName.ToString(), config.streamerBotActionArgument.ToString()).Wait();
        }
    }
}
