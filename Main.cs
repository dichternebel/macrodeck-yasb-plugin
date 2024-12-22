using SuchByte.MacroDeck;
using SuchByte.MacroDeck.Plugins;
using dichternebel.YaSB.MacroDeckPlug;
using SuchByte.MacroDeck.GUI.CustomControls;

namespace dichternebel.YaSB
{
    public class Main : MacroDeckPlugin
    {
        public static Main Instance { get; private set; }
        public static Model Model { get; private set; }
        public ContentSelectorButton Button { get; private set; }

        public Main()
        {
            Instance ??= this;
            Button = new ContentSelectorButton
            {
                BackgroundImageLayout = ImageLayout.Stretch,
                BackgroundImage = Properties.Resources.streamerbot_logo_white
            };
            Model = new Model();
            Model.PropertyChanged += Model_PropertyChanged;
        }

        private void Model_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Model.IsConnectedToStreamerBot))
            {
                Button.BackgroundImage = Model.IsConnectedToStreamerBot
                    ? Properties.Resources.streamerbot_logo_white_checked
                    : Properties.Resources.streamerbot_logo_white_error;
            }
        }

        public override bool CanConfigure => true;

        public override void Enable()
        {
            Actions = new()
            {
                new YaSBAction()
            };
            AppDomain.CurrentDomain.ProcessExit += Application_ApplicationExit;
            MacroDeck.OnMainWindowLoad += MacroDeck_OnMainWindowLoad;

            Model.InstantiateWebSocketClient();
            Model.WebSocketClient.StartConnectionAsync().Wait();
        }

        private void DisplayButton()
        {
            if (MacroDeck.MainWindow == null) return;

            // When closing the main window Button is disposed
            if (Button == null || Button.IsDisposed)
            {
                Button = new ContentSelectorButton
                {
                    BackgroundImageLayout = ImageLayout.Stretch,
                    BackgroundImage = Model.IsConnectedToStreamerBot
                    ? Properties.Resources.streamerbot_logo_white_checked
                    : Properties.Resources.streamerbot_logo_white
                };
            }

            if (!MacroDeck.MainWindow.contentButtonPanel.Controls.Contains(Button))
            {
                MacroDeck.MainWindow.contentButtonPanel.Controls.Add(Button);
                Button.Click += ModelButton_Click;
            }
        }

        private void MacroDeck_OnMainWindowLoad(object? sender, EventArgs e)
        {
            DisplayButton();
        }

        private void ModelButton_Click(object? sender, EventArgs e)
        {
            OpenConfigurator();
        }

        private void Application_ApplicationExit(object? sender, EventArgs e)
        {
            Model.WebSocketClient.Dispose();
            if (Model.IsDeleteVariablesOnExit) Model.DeleteVariables();
        }

        public override void OpenConfigurator()
        {
            DisplayButton();
            using (var dialogForm = new ConfigurationDialogForm(Model))
            {
                dialogForm.ShowDialog();
            }
        }
    }
}
