﻿using SuchByte.MacroDeck.GUI.CustomControls;

namespace dichternebel.YaSB.MacroDeckPlug
{
    public partial class ConfigurationDialogForm : DialogForm
    {
        private readonly BindingSource _bindingSource = new BindingSource();

        public Model Model
        {
            get => (Model)_bindingSource.DataSource;
            set => _bindingSource.DataSource = value;
        }

        public ConfigurationDialogForm(Model model)
        {
            InitializeComponent();
            _bindingSource.DataSource = model;

            Model.PropertyChanged += Model_PropertyChanged;

            // Add databindings to controls
            textBoxAddress.DataBindings.Add("Text", _bindingSource, nameof(Model.WebSocketHost));
            textBoxPort.DataBindings.Add("Text", _bindingSource, nameof(Model.WebSocketPort));
            textBoxEndpoint.DataBindings.Add("Text", _bindingSource, nameof(Model.WebSocketEndpoint));

            // This is not workin and I don't know why...
            //checkBox1.DataBindings.Add("Checked", _bindingSource, nameof(Model.WebSocketAuthenticationEnabled));
            checkBox1.Checked = Model.WebSocketAuthenticationEnabled;
            
            textBoxPassword.DataBindings.Add("Enabled", _bindingSource, nameof(Model.WebSocketAuthenticationEnabled));
            textBoxPassword.DataBindings.Add("Text", _bindingSource, nameof(Model.WebSocketPassword));
            // ToDo:
            //textBoxPassword.DataBindings.Add("PasswordChar", _bindingSource, "MakeVisible");

            checkBox1.CheckedChanged += (s, e) =>
            {
                Model.WebSocketAuthenticationEnabled = checkBox1.Checked;
            };
            checkBox2.CheckedChanged += (s, e) =>
            {
                buttonPrimary1.Enabled = checkBox2.Checked;
                buttonPrimary1.ForeColor = checkBox2.Checked ? Color.White : Color.DarkGray;
            };
            buttonPrimary1.Click += (s, e) =>
            {
                Model.ResetConfiguration();
                checkBox2.Checked = false;
                RefreshTreeView();
            };
            checkBox3.Checked = Model.IsDeleteVariablesOnExit;
            checkBox3.CheckedChanged += (s, e) =>
            {
                Model.IsDeleteVariablesOnExit = checkBox3.Checked;
            };

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
                    Model.SaveEvent(Helper.CreateEventKey(e.Node.Parent.Text, e.Node.Text), e.Node.Checked);
                }
            };
        }

        private void Model_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "StreamerBotEvents" && this.IsHandleCreated)
            {
                if (Model.IsConnectedToStreamerBot)
                {
                    this.BeginInvoke(() =>
                    {
                        RefreshTreeView();
                    });
                }
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
                    childNode.Checked = Model.IsEventChecked(Helper.CreateEventKey(eventGroup.Key, eventName));
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
                Model.SaveEvent(Helper.CreateEventKey(e.Node.Parent.Text, e.Node.Text), e.Node.Checked);
            }
            else
            {
                // Root node was checked/unchecked - update children
                foreach (TreeNode child in e.Node.Nodes)
                {
                    child.Checked = e.Node.Checked;
                    Model.SaveEvent(Helper.CreateEventKey(child.Parent.Text, child.Text), e.Node.Checked);
                }
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
    }
}
