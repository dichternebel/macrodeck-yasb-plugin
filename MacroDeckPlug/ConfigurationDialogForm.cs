using SuchByte.MacroDeck.GUI.CustomControls;

namespace dichternebel.YaSB.MacroDeckPlug
{
    public partial class ConfigurationDialogForm : DialogForm
    {
        private TransformationsControl transformationsControl;

        public Model Model { get; private set; }

        public ConfigurationDialogForm(Model model)
        {
            InitializeComponent();
            Model = model;
            Model.PropertyChanged += Model_PropertyChanged;

            // Add databindings to controls on Tab1

            // Websocket server settings
            textBoxAddress.DataBindings.Add("Text", Model, nameof(Model.WebSocketHost));
            textBoxPort.DataBindings.Add("Text", Model, nameof(Model.WebSocketPort));
            textBoxEndpoint.DataBindings.Add("Text", Model, nameof(Model.WebSocketEndpoint));
            checkBox1.Checked = Model.WebSocketAuthenticationEnabled;
            textBoxPassword.DataBindings.Add("Enabled", Model, nameof(Model.WebSocketAuthenticationEnabled));
            textBoxPassword.DataBindings.Add("Text", Model, nameof(Model.WebSocketPassword));
            linkLabel1.DataBindings.Add("Enabled", Model, nameof(Model.WebSocketAuthenticationEnabled));
            linkLabel1.MouseDown += (s, e) => textBoxPassword.PasswordChar = false;
            linkLabel1.MouseUp += (s, e) => textBoxPassword.PasswordChar = true;

            // Click guard for reset configuration
            checkBox1.CheckedChanged += (s, e) => Model.WebSocketAuthenticationEnabled = checkBox1.Checked;
            checkBox2.CheckedChanged += (s, e) =>
            {
                buttonPrimary1.Enabled = checkBox2.Checked;
                buttonPrimary1.ForeColor = checkBox2.Checked ? Color.White : Color.DarkGray;
            };

            // Reset configuration and rebind controls
            buttonPrimary1.Click += (s, e) =>
            {
                Model.ResetConfiguration();
                checkBox2.Checked = false;
                RefreshTreeView();
                BindTransformationControl();
            };

            // Delete variables on exit
            checkBox3.Checked = Model.IsDeleteVariablesOnExit;
            checkBox3.CheckedChanged += (s, e) => Model.IsDeleteVariablesOnExit = checkBox3.Checked;

            // Events TreeView on Tab2
            treeView1.CheckBoxes = true;
            treeView1.HideSelection = true;
            treeView1.BeforeSelect += (s, e) => e.Cancel = true;
            PopulateTreeView();
            treeView1.AfterCheck += TreeView1_AfterCheck;
            treeView1.NodeMouseClick += (s, e) => {
                TreeViewHitTestInfo hitTest = treeView1.HitTest(e.Location);
                if (e.Node != null && e.Node.Parent != null && hitTest.Location != TreeViewHitTestLocations.StateImage)
                {
                    e.Node.Checked = !e.Node.Checked;
                    Model.SetEventSubscription(Helper.CreateEventKey(e.Node.Parent.Text, e.Node.Text), e.Node.Checked);
                    UpdateParentNodeCheckState(e.Node.Parent);
                }
            };

            // Transformations DataTable on Tab3
            transformationsControl = new TransformationsControl
            {
                Dock = DockStyle.Fill
            };
            tabPage3.HorizontalScroll.Enabled = false;
            tabPage3.HorizontalScroll.Visible = false;
            tabPage3.Controls.Add(transformationsControl);

            BindTransformationControl();
            transformationsControl.Disposed += (s, e) => Model.SaveTransformations(transformationsControl.BindingList);
        }

        private void Model_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Model.StreamerBotEvents) && this.IsHandleCreated)
            {
                if (Model.IsConnectedToStreamerBot) this.BeginInvoke(RefreshTreeView);
            }
        }

        private void RefreshTreeView()
        {
            treeView1.AfterCheck -= TreeView1_AfterCheck;
            PopulateTreeView();
            treeView1.AfterCheck += TreeView1_AfterCheck;
        }

        private void PopulateTreeView()
        {
            if (Model.StreamerBotEvents == null) return;

            treeView1.Nodes.Clear();

            // Group events by their group property and order alphabetically
            var orderedEvents = Model.StreamerBotEvents.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);

            // Create nodes for each dictionary entry
            foreach (var eventGroup in orderedEvents)
            {
                // Add the dictionary key as root node
                TreeNode rootNode = treeView1.Nodes.Add(eventGroup.Key);

                // Add all array items as child nodes
                foreach (var eventName in eventGroup.Value)
                {
                    TreeNode childNode = rootNode.Nodes.Add(eventName);
                    childNode.Checked = Model.IsEventSubscribed(Helper.CreateEventKey((string)eventGroup.Key, (string)eventName));
                }
                UpdateParentNodeCheckState(rootNode);
            }
        }

        private void TreeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            // Prevent infinite recursion
            if (e.Action == TreeViewAction.Unknown) return;

            if (e.Node.Parent != null)
            {
                // Child node was checked/unchecked - update parent
                UpdateParentNodeCheckState(e.Node.Parent);
                Model.SetEventSubscription(Helper.CreateEventKey(e.Node.Parent.Text, e.Node.Text), e.Node.Checked);
            }
            else
            {
                // Root node was checked/unchecked - update children
                var eventList = new List<string>();
                foreach (TreeNode child in e.Node.Nodes)
                {
                    child.Checked = e.Node.Checked;
                    //Model.SaveEvent(Helper.CreateEventKey(child.Parent.Text, child.Text), e.Node.Checked);
                    eventList.Add(Helper.CreateEventKey(child.Parent.Text, child.Text));
                }
                Model.SetEventSubscriptions(eventList, e.Node.Checked);
            }
        }

        private void UpdateParentNodeCheckState(TreeNode parentNode)
        {
            treeView1.AfterCheck -= TreeView1_AfterCheck;
            var allChecked = parentNode.Nodes.Cast<TreeNode>().All(n => n.Checked);
            var anyChecked = parentNode.Nodes.Cast<TreeNode>().Any(n => n.Checked);

            if (allChecked)
            {
                parentNode.Checked = true;
            }
            else if (anyChecked)
            {
                parentNode.Checked = true;
            }
            else
            {
                parentNode.Checked = false;
            }
            treeView1.AfterCheck += TreeView1_AfterCheck;
        }

        private void BindTransformationControl()
        {
            var currentTransformations = Model.GetTransformations();
            transformationsControl.BindToTransformations(currentTransformations);
        }
    }
}
