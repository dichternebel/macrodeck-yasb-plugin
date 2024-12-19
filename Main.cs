using SuchByte.MacroDeck;
using SuchByte.MacroDeck.GUI;
using SuchByte.MacroDeck.Plugins;
using dichternebel.YaSB.MacroDeckPlug;

namespace dichternebel.YaSB
{
    public class Main : MacroDeckPlugin
    {
        public static Main Instance { get; private set; }
        public static Model Model { get; private set; }
        public static bool IsButtonDisplayed { get; private set; }

        public Main()
        {
            Instance ??= this;
            Model = new Model();
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
            if (IsButtonDisplayed || MacroDeck.MainWindow == null) return;
            MacroDeck.MainWindow.contentButtonPanel.Controls.Add(Model.Button);
            Model.Button.Click += ModelButton_Click;
            IsButtonDisplayed = true;
        }

        private void MacroDeck_OnMainWindowLoad(object? sender, EventArgs e)
        {
            var mainWindow = sender as MainWindow;
            if (mainWindow == null) return;
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
