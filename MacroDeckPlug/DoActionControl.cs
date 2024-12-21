using System.Text.Json;
using SuchByte.MacroDeck.GUI;
using SuchByte.MacroDeck.GUI.CustomControls;
using SuchByte.MacroDeck.Plugins;

namespace dichternebel.YaSB.MacroDeckPlug
{
    public partial class DoActionControl : ActionConfigControl
    {
        private PluginAction _macroDeckAction;

        private ActionConfiguration config {  get; set; }

        public DoActionControl(PluginAction macroDeckAction, ActionConfigurator actionConfigurator)
        {
            this._macroDeckAction = macroDeckAction;
            InitializeComponent();

            config = new ActionConfiguration();

            if (_macroDeckAction.Configuration != null)
            {
                config = JsonSerializer.Deserialize<ActionConfiguration>(_macroDeckAction.Configuration);
                if (config != null)
                {
                    roundedTextBoxActionId.Text = config.streamerBotActionId;
                    roundedTextBoxActionName.Text = config.streamerBotActionName;
                    roundedTextBoxArgument.Text = config.streamerBotActionArgument;
                }
            }

            var groups = Main.Model.StreamerBotActions?
                .Select(x => string.IsNullOrEmpty(x.Group) ? "None" : x.Group)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            if (groups?.Count > 0)
            {
                comboBox1.SelectedIndexChanged -= ComboBox1_SelectedIndexChanged;
                comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;
                comboBox1.DataSource = groups;
                comboBox1.SelectedIndex = 0;
            }
        }


        private void ComboBox1_SelectedIndexChanged(object? sender, EventArgs e)
        {
            var selectedGroup = comboBox1.SelectedItem?.ToString();
            var filteredActions = Main.Model.StreamerBotActions?
                .Where(x => selectedGroup == "None" ?
                    string.IsNullOrEmpty(x.Group) :
                    x.Group == selectedGroup)
                .OrderBy(x => x.Name)
                .ToList();

            if (filteredActions?.Count > 0)
            {
                comboBox2.DataSource = filteredActions;
                comboBox2.DisplayMember = "Name";
                comboBox2.ValueMember = "Id";
                comboBox2.SelectedIndex = 0;
            }
        }

        private void buttonPrimary1_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem == null) return;

            roundedTextBoxActionId.Text = ((StreamerBot.Action)comboBox2.SelectedItem).Id;
            roundedTextBoxActionName.Text = ((StreamerBot.Action)comboBox2.SelectedItem).Name;
        }

        public override bool OnActionSave()
        {
            if (string.IsNullOrWhiteSpace(roundedTextBoxActionId.Text)
                || string.IsNullOrWhiteSpace(roundedTextBoxActionName.Text)) return false;

            if (roundedTextBoxActionId.Text == config.streamerBotActionId
                && roundedTextBoxActionName.Text == config.streamerBotActionName
                && roundedTextBoxArgument.Text == config.streamerBotActionArgument) return true;

            config.streamerBotActionId = roundedTextBoxActionId.Text;
            config.streamerBotActionName = roundedTextBoxActionName.Text;
            config.streamerBotActionArgument = roundedTextBoxArgument.Text;

            string configJson = JsonSerializer.Serialize(config);

            this._macroDeckAction.ConfigurationSummary = $":: {roundedTextBoxActionName.Text} :: {roundedTextBoxArgument.Text}";
            this._macroDeckAction.Configuration = configJson.ToString();

            return true;
        }
    }
}
